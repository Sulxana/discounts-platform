using Discounts.Application.Reservations.Interfaces;
using Discounts.Domain.Reservations;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DiscountsDbContext _context;

        public ReservationRepository(DiscountsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CancellationToken token, Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation, token);
        }

        public async Task<Reservation?> GetByIdAsync(CancellationToken token, Guid id)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id, token);
        }
        public async Task<List<(Reservation Reservation, string OfferTitle, decimal Price)>> GetUserActiveReservationsWithOffersAsync(Guid userId,CancellationToken token)
        {
            var results = await _context.Reservations
                .Where(r => r.UserId == userId && r.Status == ReservationStatus.Active)
                .Join(_context.Offer,
                    reservation => reservation.OfferId,
                    offer => offer.Id,
                    (reservation, offer) => new { Reservation = reservation, OfferTitle = offer.Title, Price = offer.DiscountedPrice })
                .OrderByDescending(x => x.Reservation.CreatedAt)
                .ToListAsync(token);

            return results.Select(x => (x.Reservation, x.OfferTitle, x.Price)).ToList();
        }
        public async Task<List<Reservation>> GetExpiredActiveAsync(CancellationToken token)
        {
            var now = DateTime.UtcNow;
            return await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Active && r.ExpiresAt < now)
                .ToListAsync(token); ;
        }

        public async Task<bool> HasActiveReservationForOfferAsync(Guid userId, Guid offerId, CancellationToken token)
        {
            return await _context.Reservations
                .AnyAsync(r => r.UserId == userId &&
                r.OfferId == offerId && r.Status == ReservationStatus.Active,token);
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await _context.SaveChangesAsync(token);
        }
    }
}
