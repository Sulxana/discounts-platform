using Discounts.Domain.Reservations;

namespace Discounts.Application.Reservations.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(CancellationToken token, Guid id);
        Task<List<(Reservation Reservation, string OfferTitle, decimal Price)>> GetUserActiveReservationsWithOffersAsync(Guid userId, CancellationToken token);
        Task AddAsync(CancellationToken token, Reservation reservation);
        Task<bool> HasActiveReservationForOfferAsync(Guid userId, Guid offerId, CancellationToken ct);
        Task<List<Reservation>> GetExpiredActiveAsync(CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
