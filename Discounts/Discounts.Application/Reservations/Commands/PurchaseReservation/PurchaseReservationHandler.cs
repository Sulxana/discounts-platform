using Discounts.Application.Common.Interfaces;
using Discounts.Application.Reservations.Interfaces;
using Discounts.Domain.Coupons;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Discounts.Application.Reservations.Commands.PurchaseReservation
{
    public class PurchaseReservationHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PurchaseReservationHandler> _logger;
        private readonly ICouponRepository _couponRepository;
        private readonly IOfferRepository _offerRepository;

        public PurchaseReservationHandler(IReservationRepository reservationRepository, IUnitOfWork unitOfWork, ILogger<PurchaseReservationHandler> logger, ICouponRepository couponRepository, IOfferRepository offerRepository)
        {
            _reservationRepository = reservationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _couponRepository = couponRepository;
            _offerRepository = offerRepository;
        }

        public async Task<List<Guid>> Handle(PurchaseReservationCommand command, CancellationToken token)
        {
            var reservation = await _reservationRepository.GetByIdAsync(token, command.ReservationId);

            if (reservation == null)
                throw new KeyNotFoundException($"Reservation {command.ReservationId} not found.");

            if (!reservation.IsActive())
                throw new InvalidOperationException($"Reservation {command.ReservationId} is not active. Status: {reservation.Status}");

            if (reservation.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException($"Reservation {command.ReservationId} has expired.");

            reservation.MarkAsCompleted();

            var offer = await _offerRepository.GetOfferByIdAsync(token, reservation.OfferId);
            if (offer == null)
                throw new KeyNotFoundException($"Offer {reservation.OfferId} not found.");

            var coupons = new List<Coupon>();
            for (int i = 0; i < reservation.Quantity; i++)
            {
                var coupon = new Coupon(reservation.UserId, reservation.OfferId, reservation.Id, offer.EndDate);
                coupons.Add(coupon);
            }

            await _couponRepository.AddRangeAsync(token, coupons);

            await _unitOfWork.SaveChangesAsync(token);

            _logger.LogInformation($"Successfully purchased reservation {command.ReservationId}. Created {coupons.Count} coupons.");

            return coupons.Select(c => c.Id).ToList();
        }
    }
}
