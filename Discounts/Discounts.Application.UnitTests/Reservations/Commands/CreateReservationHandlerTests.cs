using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Commands.CreateReservation;
using Discounts.Application.Reservations.Interfaces;
using Discounts.Application.Settings.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Domain.Reservations;
using Discounts.Domain.Settings;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Discounts.Application.UnitTests.Reservations.Commands
{
    public class CreateReservationHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IOfferRepository> _offerRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IValidator<CreateReservationCommand>> _validatorMock;
        private readonly Mock<IGlobalSettingsService> _settingsServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateReservationHandler _handler;

        public CreateReservationHandlerTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _offerRepositoryMock = new Mock<IOfferRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _validatorMock = new Mock<IValidator<CreateReservationCommand>>();
            _settingsServiceMock = new Mock<IGlobalSettingsService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CreateReservationHandler(
                _reservationRepositoryMock.Object,
                _offerRepositoryMock.Object,
                _currentUserServiceMock.Object,
                _validatorMock.Object,
                _settingsServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        private static Offer CreateValidOffer(OfferStatus status = OfferStatus.Approved, int remainingCoupons = 10, int expirationDays = 7)
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
                DateTime.UtcNow.AddDays(expirationDays),
                Guid.NewGuid()
            );

            var remainingCouponsProp = typeof(Offer).GetProperty("RemainingCoupons");
            remainingCouponsProp?.SetValue(offer, remainingCoupons);

            var statusProp = typeof(Offer).GetProperty("Status");
            statusProp?.SetValue(offer, status);

            return offer;
        }

        [Fact]
        public async Task CreateReservation_ShouldSucceed_WhenCommandIsValid()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateReservationCommand { OfferId = offerId, Quantity = 2 };
            var offer = CreateValidOffer(OfferStatus.Approved, 10, 7);

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);
            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            _reservationRepositoryMock.Setup(r => r.HasActiveReservationForOfferAsync(userId, offerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _settingsServiceMock.Setup(s => s.GetIntAsync(SettingKeys.ReservationExpirationMinutes, 30, It.IsAny<CancellationToken>()))
                .ReturnsAsync(30);

            // Act
            var result = await _handler.CreateReservation(CancellationToken.None, command);

            // Assert
            result.Should().NotBeEmpty();
            offer.RemainingCoupons.Should().Be(8); // 10 - 2
            _reservationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CancellationToken>(), It.Is<Reservation>(res => res.UserId == userId && res.OfferId == offerId && res.Quantity == 2)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateReservation_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var command = new CreateReservationCommand { OfferId = Guid.NewGuid(), Quantity = 2 };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _currentUserServiceMock.Setup(c => c.UserId).Returns((Guid?)null);

            // Act
            var act = async () => await _handler.CreateReservation(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User must be authenticated");
            _reservationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CancellationToken>(), It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateReservation_ShouldThrowInvalidOperationException_WhenOfferDoesNotExist()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new CreateReservationCommand { OfferId = offerId, Quantity = 2 };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync((Offer?)null);

            // Act
            var act = async () => await _handler.CreateReservation(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Offer not found");
        }

        [Fact]
        public async Task CreateReservation_ShouldThrowInvalidOperationException_WhenOfferIsNotApproved()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new CreateReservationCommand { OfferId = offerId, Quantity = 2 };
            var offer = CreateValidOffer(OfferStatus.Pending, 10, 7);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.CreateReservation(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Only approved offers can be reserved");
        }

        [Fact]
        public async Task CreateReservation_ShouldThrowInvalidOperationException_WhenOfferIsExpired()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new CreateReservationCommand { OfferId = offerId, Quantity = 2 };
            var offer = CreateValidOffer(OfferStatus.Approved, 10, -1); // Expired 1 day ago

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _currentUserServiceMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.CreateReservation(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("This offer has expired");
        }

        [Fact]
        public async Task CreateReservation_ShouldThrowInvalidOperationException_WhenUserHasActiveReservation()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateReservationCommand { OfferId = offerId, Quantity = 2 };
            var offer = CreateValidOffer(OfferStatus.Approved, 10, 7);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);
            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            _reservationRepositoryMock.Setup(r => r.HasActiveReservationForOfferAsync(userId, offerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Already has reservation

            // Act
            var act = async () => await _handler.CreateReservation(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("You already have an active reservation for this offer");
        }

        [Fact]
        public async Task CreateReservation_ShouldThrowInvalidOperationException_WhenNotEnoughCoupons()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateReservationCommand { OfferId = offerId, Quantity = 5 };
            var offer = CreateValidOffer(OfferStatus.Approved, 3, 7); // Only 3 left

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);
            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            _reservationRepositoryMock.Setup(r => r.HasActiveReservationForOfferAsync(userId, offerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _handler.CreateReservation(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("only 3 coupons are available");
        }

        [Fact]
        public async Task CreateReservation_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateReservationCommand { OfferId = Guid.Empty, Quantity = 0 };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("OfferId", "Required") };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.CreateReservation(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _reservationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CancellationToken>(), It.IsAny<Reservation>()), Times.Never);
        }
    }
}
