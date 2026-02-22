using Discounts.Application.Common.Interfaces;
using Discounts.Application.Coupons.Commands.DirectPurchase;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Coupons;
using Discounts.Domain.Offers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Discounts.Application.UnitTests.Coupons.Commands
{
    public class DirectPurchaseHandlerTests
    {
        private readonly Mock<IOfferRepository> _offerRepositoryMock;
        private readonly Mock<ICouponRepository> _couponRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DirectPurchaseHandler>> _loggerMock;
        private readonly Mock<IValidator<DirectPurchaseCommand>> _validatorMock;
        private readonly DirectPurchaseHandler _handler;

        public DirectPurchaseHandlerTests()
        {
            _offerRepositoryMock = new Mock<IOfferRepository>();
            _couponRepositoryMock = new Mock<ICouponRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DirectPurchaseHandler>>();
            _validatorMock = new Mock<IValidator<DirectPurchaseCommand>>();

            _handler = new DirectPurchaseHandler(
                _offerRepositoryMock.Object,
                _couponRepositoryMock.Object,
                _currentUserServiceMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _validatorMock.Object);
        }

        private static Offer CreateOffer(Guid offerId, OfferStatus status, int remainingCoupons, DateTime expiresAt)
        {
            var offer = new Offer("Title", "Desc", Guid.NewGuid(), null, 10m, 5m, remainingCoupons + 10, DateTime.UtcNow, expiresAt, Guid.NewGuid());
            var idProp = typeof(Offer).GetProperty("Id");
            idProp?.SetValue(offer, offerId);

            var statusProp = typeof(Offer).GetProperty("Status");
            statusProp?.SetValue(offer, status);

            var remainingCouponsProp = typeof(Offer).GetProperty("RemainingCoupons");
            remainingCouponsProp?.SetValue(offer, remainingCoupons);
            return offer;
        }

        [Fact]
        public async Task Handle_ShouldPurchaseCoupons_WhenCommandIsValid()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var quantity = 3;
            var command = new DirectPurchaseCommand { OfferId = offerId, Quantity = quantity };
            var offer = CreateOffer(offerId, OfferStatus.Approved, 10, DateTime.UtcNow.AddDays(1));

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DirectPurchaseCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().HaveCount(quantity);
            offer.RemainingCoupons.Should().Be(7); // 10 - 3

            _couponRepositoryMock.Verify(c => c.AddRangeAsync(It.IsAny<CancellationToken>(), It.Is<IEnumerable<Coupon>>(list => list.Count() == quantity)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new DirectPurchaseCommand { OfferId = Guid.Empty, Quantity = 0 };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("OfferId", "Required") };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DirectPurchaseCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _couponRepositoryMock.Verify(c => c.AddRangeAsync(It.IsAny<CancellationToken>(), It.IsAny<IEnumerable<Coupon>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var command = new DirectPurchaseCommand { OfferId = Guid.NewGuid(), Quantity = 2 };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DirectPurchaseCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns((Guid?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User must be authenticated to purchase.");
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenOfferIsNotFound()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new DirectPurchaseCommand { OfferId = offerId, Quantity = 2 };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DirectPurchaseCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync((Offer?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Offer {offerId} not found.");
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenOfferIsNotApproved()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new DirectPurchaseCommand { OfferId = offerId, Quantity = 2 };
            var offer = CreateOffer(offerId, OfferStatus.Pending, 10, DateTime.UtcNow.AddDays(1));

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DirectPurchaseCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Offer {offer.Title} is not approved for purchase.");
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenOfferIsExpired()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new DirectPurchaseCommand { OfferId = offerId, Quantity = 2 };

            // Create offer that expired an hour ago
            var offer = CreateOffer(offerId, OfferStatus.Approved, 10, DateTime.UtcNow.AddHours(-1));

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DirectPurchaseCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Offer {offer.Title} has expired.");
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentOutOfRangeException_WhenNotEnoughCoupons()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new DirectPurchaseCommand { OfferId = offerId, Quantity = 100 };

            // Only 10 coupons left
            var offer = CreateOffer(offerId, OfferStatus.Approved, 10, DateTime.UtcNow.AddDays(1));

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DirectPurchaseCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}
