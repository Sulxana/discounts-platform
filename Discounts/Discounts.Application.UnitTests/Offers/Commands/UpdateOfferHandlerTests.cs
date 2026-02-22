using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Settings.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Domain.Settings;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Discounts.Application.UnitTests.Offers.Commands
{
    public class UpdateOfferHandlerTests
    {
        private readonly Mock<IOfferRepository> _repositoryMock;
        private readonly Mock<IValidator<UpdateOfferCommand>> _validatorMock;
        private readonly Mock<IGlobalSettingsService> _settingsServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly UpdateOfferHandler _handler;

        public UpdateOfferHandlerTests()
        {
            _repositoryMock = new Mock<IOfferRepository>();
            _validatorMock = new Mock<IValidator<UpdateOfferCommand>>();
            _settingsServiceMock = new Mock<IGlobalSettingsService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new UpdateOfferHandler(
                _repositoryMock.Object,
                _validatorMock.Object,
                _settingsServiceMock.Object,
                _currentUserServiceMock.Object);
        }

        private static Offer CreateValidOffer(Guid merchantId, DateTime createdAt)
        {
            var offer = new Offer(
                "Test Offer",
                "Description",
                Guid.NewGuid(),
                null,
                100m,
                50m,
                10,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7),
                merchantId
            );

            // Using reflection to set CreatedAt manually since we need to test edit window 
            // and constructor sets it to DateTime.UtcNow
            var createdAtProperty = typeof(Offer).GetProperty("CreatedAt");
            createdAtProperty?.SetValue(offer, createdAt);

            return offer;
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldUpdateFields_WhenCommandIsValidAndUserIsOwner()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var command = new UpdateOfferCommand { Id = offerId, Title = "New Title", DiscountedPrice = 40m };
            var offer = CreateValidOffer(merchantId, DateTime.UtcNow);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);
            _currentUserServiceMock.Setup(c => c.UserId).Returns(merchantId);
            _settingsServiceMock.Setup(s => s.GetIntAsync(SettingKeys.MerchantEditWindowHours, 24, It.IsAny<CancellationToken>()))
                .ReturnsAsync(24);

            // Act
            await _handler.UpdateOfferAsync(CancellationToken.None, command);

            // Assert
            offer.Title.Should().Be("New Title");
            offer.DiscountedPrice.Should().Be(40m);
            _repositoryMock.Verify(r => r.UpdateOfferAsync(It.IsAny<CancellationToken>(), offer), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var command = new UpdateOfferCommand { Id = offerId, Title = "New Title" };
            var offer = CreateValidOffer(merchantId, DateTime.UtcNow);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            _currentUserServiceMock.Setup(c => c.UserId).Returns(otherUserId);
            _currentUserServiceMock.Setup(c => c.IsInRole(Roles.Administrator)).Returns(false);

            // Act
            var act = async () => await _handler.UpdateOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _repositoryMock.Verify(r => r.UpdateOfferAsync(It.IsAny<CancellationToken>(), It.IsAny<Offer>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldThrowInvalidOperationException_WhenEditWindowIsExpired()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var command = new UpdateOfferCommand { Id = offerId, Title = "New Title" };

            // Created 25 hours ago
            var offer = CreateValidOffer(merchantId, DateTime.UtcNow.AddHours(-25));

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            _currentUserServiceMock.Setup(c => c.UserId).Returns(merchantId);
            _settingsServiceMock.Setup(s => s.GetIntAsync(SettingKeys.MerchantEditWindowHours, 24, It.IsAny<CancellationToken>()))
                .ReturnsAsync(24); // Window is 24 hours

            // Act
            var act = async () => await _handler.UpdateOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot edit offer after 24 hours from creation");
            _repositoryMock.Verify(r => r.UpdateOfferAsync(It.IsAny<CancellationToken>(), It.IsAny<Offer>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldThrowValidationException_WhenAtLeastOneFieldIsNotProvided()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var command = new UpdateOfferCommand { Id = offerId }; // Empty fields
            var offer = CreateValidOffer(merchantId, DateTime.UtcNow);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            _currentUserServiceMock.Setup(c => c.UserId).Returns(merchantId);
            _settingsServiceMock.Setup(s => s.GetIntAsync(SettingKeys.MerchantEditWindowHours, 24, It.IsAny<CancellationToken>()))
                .ReturnsAsync(24);

            // Act
            var act = async () => await _handler.UpdateOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("At least one field must be provided for update.");
            _repositoryMock.Verify(r => r.UpdateOfferAsync(It.IsAny<CancellationToken>(), It.IsAny<Offer>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOfferAsync_ShouldThrowValidationException_WhenDiscountedPriceExceedsOriginalPrice()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var command = new UpdateOfferCommand { Id = offerId, DiscountedPrice = 110m };
            var offer = CreateValidOffer(merchantId, DateTime.UtcNow); // Original price is 100

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            _currentUserServiceMock.Setup(c => c.UserId).Returns(merchantId);
            _settingsServiceMock.Setup(s => s.GetIntAsync(SettingKeys.MerchantEditWindowHours, 24, It.IsAny<CancellationToken>()))
                .ReturnsAsync(24);

            // Act
            var act = async () => await _handler.UpdateOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("DiscountedPrice must be less than OriginalPrice.");
        }
    }
}
