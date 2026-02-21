using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Mapster;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Discounts.Application.UnitTests.Offers.Commands
{
    public class CreateOfferHandlerTests
    {
        private readonly Mock<IOfferRepository> _repositoryMock;
        private readonly Mock<IValidator<CreateOfferCommand>> _validatorMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateOfferHandler _handler;

        public CreateOfferHandlerTests()
        {
            _repositoryMock = new Mock<IOfferRepository>();
            _validatorMock = new Mock<IValidator<CreateOfferCommand>>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            TypeAdapterConfig<CreateOfferCommand, Offer>.NewConfig()
                .ConstructUsing(src => new Offer(
                    src.Title,
                    src.Description,
                    src.CategoryId,
                    src.ImageUrl,
                    src.OriginalPrice,
                    src.DiscountedPrice,
                    src.TotalCoupons,
                    src.StartDate,
                    src.EndDate,
                    Guid.Empty
                ));

            _handler = new CreateOfferHandler(
                _repositoryMock.Object,
                _validatorMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task CreateOffer_ShouldReturnOfferId_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreateOfferCommand
            {
                Title = "Test Offer",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                OriginalPrice = 100m,
                DiscountedPrice = 50m,
                TotalCoupons = 10,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            var userId = Guid.NewGuid();

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            _repositoryMock
                .Setup(r => r.AddOfferAsync(It.IsAny<CancellationToken>(), It.IsAny<Offer>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.CreateOffer(CancellationToken.None, command);

            // Assert
            result.Should().NotBeEmpty();
            _repositoryMock.Verify(r => r.AddOfferAsync(It.IsAny<CancellationToken>(), It.Is<Offer>(o => o.MerchantId == userId && o.Title == command.Title)), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateOffer_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateOfferCommand();
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Title", "Title is required") };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.CreateOffer(CancellationToken.None, command);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _repositoryMock.Verify(r => r.AddOfferAsync(It.IsAny<CancellationToken>(), It.IsAny<Offer>()), Times.Never);
        }

        [Fact]
        public async Task CreateOffer_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var command = new CreateOfferCommand
            {
                Title = "Test Offer"
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns((Guid?)null);

            // Act
            var act = async () => await _handler.CreateOffer(CancellationToken.None, command);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _repositoryMock.Verify(r => r.AddOfferAsync(It.IsAny<CancellationToken>(), It.IsAny<Offer>()), Times.Never);
        }
    }
}
