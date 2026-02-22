using Discounts.Application.Common.Interfaces;
using Discounts.Application.Reservations.Interfaces;

namespace Discounts.Application.Reservations.Queries.GetUserReservations
{
    public class GetUserReservationsHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetUserReservationsHandler(IReservationRepository reservationRepository, ICurrentUserService currentUserService)
        {
            _reservationRepository = reservationRepository;
            _currentUserService = currentUserService;
        }
        public async Task<List<ReservationDto>> GetUserReservations(CancellationToken token, GetUserReservationsQuery query)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User must be authenticated");

            var reservations = await _reservationRepository.GetUserActiveReservationsWithOffersAsync(userId.Value, token).ConfigureAwait(false);

            return reservations.Select(r => new ReservationDto
            {
                Id = r.Reservation.Id,
                OfferId = r.Reservation.OfferId,
                OfferTitle = r.OfferTitle,
                Quantity = r.Reservation.Quantity,
                CreatedAt = r.Reservation.CreatedAt,
                ExpiresAt = r.Reservation.ExpiresAt,
                Status = r.Reservation.Status.ToString(),
                MinutesRemaining = (int)(r.Reservation.ExpiresAt - DateTime.UtcNow).TotalMinutes,
                Price = r.Price
            }).ToList();
        }
    }
}
