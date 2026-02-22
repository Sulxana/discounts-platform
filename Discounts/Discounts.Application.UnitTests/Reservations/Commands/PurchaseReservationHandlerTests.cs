using Discounts.Application.Common.Interfaces;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Commands.PurchaseReservation;
using Discounts.Application.Reservations.Interfaces;
using Discounts.Domain.Coupons;
using Discounts.Domain.Reservations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Discounts.Application.UnitTests.Reservations.Commands
{
    public class PurchaseReservationHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<PurchaseReservationHandler>> _loggerMock;
        private readonly Mock<ICouponRepository> _couponRepositoryMock;
        private readonly Mock<IOfferRepository> _offerRepositoryMock;
        private readonly PurchaseReservationHandler _handler;

        public PurchaseReservationHandlerTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<PurchaseReservationHandler>>();
            _couponRepositoryMock = new Mock<ICouponRepository>();
            _offerRepositoryMock = new Mock<IOfferRepository>();

            _handler = new PurchaseReservationHandler(
                _reservationRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _couponRepositoryMock.Object,
                _offerRepositoryMock.Object);
        }

        private static Reservation CreateReservation(Guid reservationId, int quantity, DateTime expiresAt)
        {
            var reservation = new Reservation(Guid.NewGuid(), Guid.NewGuid(), quantity, expiresAt);
            var idProp = typeof(Reservation).GetProperty("Id");
            idProp?.SetValue(reservation, reservationId);
            return reservation;
        }

        [Fact]
        public async Task Handle_ShouldPurchaseReservationAndReturnCouponIds_WhenValid()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var command = new PurchaseReservationCommand { ReservationId = reservationId };
            var reservation = CreateReservation(reservationId, 3, DateTime.UtcNow.AddMinutes(30));

            _reservationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), reservationId))
                .ReturnsAsync(reservation);

            var offerId = reservation.OfferId;
            var offer = new Discounts.Domain.Offers.Offer("Title", "Desc", Guid.NewGuid(), null, 10m, 5m, 10, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Guid.NewGuid());
            _offerRepositoryMock.Setup(o => o.GetOfferByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            reservation.IsCompleted().Should().BeTrue();

            _couponRepositoryMock.Verify(c => c.AddRangeAsync(It.IsAny<CancellationToken>(), It.Is<IEnumerable<Coupon>>(list => list.Count() == 3)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenReservationNotFound()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var command = new PurchaseReservationCommand { ReservationId = reservationId };

            _reservationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), reservationId))
                .ReturnsAsync((Reservation?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Reservation {reservationId} not found.");

            _couponRepositoryMock.Verify(c => c.AddRangeAsync(It.IsAny<CancellationToken>(), It.IsAny<IEnumerable<Coupon>>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenReservationIsNotActive()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var command = new PurchaseReservationCommand { ReservationId = reservationId };
            var reservation = CreateReservation(reservationId, 3, DateTime.UtcNow.AddMinutes(30));

            reservation.Cancel(); // Status becomes Cancelled

            _reservationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), reservationId))
                .ReturnsAsync(reservation);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Reservation {reservationId} is not active. Status: Cancelled");

            _couponRepositoryMock.Verify(c => c.AddRangeAsync(It.IsAny<CancellationToken>(), It.IsAny<IEnumerable<Coupon>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenReservationIsExpired()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var command = new PurchaseReservationCommand { ReservationId = reservationId };

            // Create valid reservation first to pass constructor validation
            var reservation = CreateReservation(reservationId, 3, DateTime.UtcNow.AddMinutes(30));

            // Backdate ExpiresAt so it simulates an expired active reservation
            var expiresAtProp = typeof(Reservation).GetProperty("ExpiresAt");
            expiresAtProp?.SetValue(reservation, DateTime.UtcNow.AddMinutes(-30));

            _reservationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<CancellationToken>(), reservationId))
                .ReturnsAsync(reservation);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Reservation {reservationId} has expired.");

            _couponRepositoryMock.Verify(c => c.AddRangeAsync(It.IsAny<CancellationToken>(), It.IsAny<IEnumerable<Coupon>>()), Times.Never);
        }
    }
}
