using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Commands.CancelReservation;
using Discounts.Application.Reservations.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Domain.Reservations;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Discounts.Application.UnitTests.Reservations.Commands
{
    public class CancelReservationHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IOfferRepository> _offerRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IValidator<CancelReservationCommand>> _validatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CancelReservationHandler _handler;

        public CancelReservationHandlerTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _offerRepositoryMock = new Mock<IOfferRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _validatorMock = new Mock<IValidator<CancelReservationCommand>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CancelReservationHandler(
                _reservationRepositoryMock.Object,
                _offerRepositoryMock.Object,
                _currentUserServiceMock.Object,
                _validatorMock.Object,
                _unitOfWorkMock.Object);
        }

        private static Reservation CreateReservation(Guid reservationId, Guid userId, Guid offerId, int quantity)
        {
            var reservation = new Reservation(userId, offerId, quantity, DateTime.UtcNow.AddMinutes(30));
            var idProp = typeof(Reservation).GetProperty("Id");
            idProp?.SetValue(reservation, reservationId);
            return reservation;
        }

        private static Offer CreateOffer(Guid offerId, int remainingCoupons)
        {
            var offer = new Offer("Title", "Desc", Guid.NewGuid(), null, 10m, 5m, remainingCoupons + 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
            var idProp = typeof(Offer).GetProperty("Id");
            idProp?.SetValue(offer, offerId);

            var remainingCouponsProp = typeof(Offer).GetProperty("RemainingCoupons");
            remainingCouponsProp?.SetValue(offer, remainingCoupons);
            return offer;
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldCancelAndIncreaseStock_WhenCommandIsValidAndUserIsOwner()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var offerId = Guid.NewGuid();
            var quantity = 2;

            var command = new CancelReservationCommand(reservationId);
            var reservation = CreateReservation(reservationId, userId, offerId, quantity);
            var offer = CreateOffer(offerId, 5); // 5 coupons remaining

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CancelReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _reservationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), reservationId))
                .ReturnsAsync(reservation);

            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId); // Match owner

            _offerRepositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            await _handler.CancelReservationAsync(CancellationToken.None, command);

            // Assert
            reservation.IsActive().Should().BeFalse();
            reservation.Status.Should().Be(ReservationStatus.Cancelled);
            offer.RemainingCoupons.Should().Be(7); // Increased by 2

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CancelReservationCommand(Guid.Empty);
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Id", "Required") };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CancelReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.CancelReservationAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _reservationRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldThrowNotFoundException_WhenReservationNotFound()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var command = new CancelReservationCommand(reservationId);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CancelReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _reservationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), reservationId))
                .ReturnsAsync((Reservation?)null);

            // Act
            var act = async () => await _handler.CancelReservationAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Reservation not found");
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwner()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var reservationOwnerId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid();

            var command = new CancelReservationCommand(reservationId);
            var reservation = CreateReservation(reservationId, reservationOwnerId, Guid.NewGuid(), 2);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CancelReservationCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _reservationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), reservationId))
                .ReturnsAsync(reservation);

            _currentUserServiceMock.Setup(c => c.UserId).Returns(differentUserId); // Different user

            // Act
            var act = async () => await _handler.CancelReservationAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("You can only cancel your own reservations.");

            _offerRepositoryMock.Verify(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}
