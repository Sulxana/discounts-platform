using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Interfaces;
using Microsoft.Extensions.Logging;

namespace Discounts.Application.Reservations.Services
{
    public class ReservationCleanupService : IReservationCleanupService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReservationCleanupService> _logger;

        public ReservationCleanupService(IReservationRepository reservationRepository, IOfferRepository offerRepository, IUnitOfWork unitOfWork, ILogger<ReservationCleanupService> logger)
        {
            _reservationRepository = reservationRepository;
            _offerRepository = offerRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> ProcessReservation(Guid reservationId, CancellationToken token)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(token, reservationId).ConfigureAwait(false);

                if (reservation == null)
                {
                    _logger.LogWarning($"Reservation {reservationId} not found.");
                    return false;
                }

                if (!reservation.IsActive())
                {
                    _logger.LogInformation($"Reservation {reservationId} is not active. Skipping.");
                    return true;
                }

                if (reservation.ExpiresAt > DateTime.UtcNow)
                {
                    _logger.LogInformation($"Reservation {reservationId} is not expired yet. Skipping.");
                    return true;
                }

                reservation.MarkAsExpired(DateTime.UtcNow);

                var offer = await _offerRepository.GetOfferForUpdateByIdAsync(token, reservation.OfferId).ConfigureAwait(false);
                if (offer != null)
                {
                    offer.IncreaseStock(reservation.Quantity);
                }
                else
                {
                    _logger.LogWarning($"Offer {reservation.OfferId} not found. Stock not restored.");
                }

                await _unitOfWork.SaveChangesAsync(token).ConfigureAwait(false);
                _logger.LogInformation($"Successfully cleaned up reservation {reservationId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cleanup reservation {reservationId}");
                throw;
            }
        }
    }
}
