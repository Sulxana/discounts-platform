using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Domain.Offers;
using Discounts.Infrastracture.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Repositories
{
    public class OfferRepository : BaseRepository<Offer>, IOfferRepository
    {
        public OfferRepository(DiscountsDbContext context) : base(context)
        {
        }

        public async Task AddOfferAsync(CancellationToken token, Offer createOffer)
        {
            await base.Add(token, createOffer);
        }

        public async Task<List<Offer>> GetActiveOfferAsync(CancellationToken token, OfferCategory? category, OfferStatus? status, int page, int pageSize)
        {
            var query = _context.Set<Offer>().AsNoTracking();

            if (category.HasValue)
                query = query.Where(x => x.Category == category.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.OrderByDescending(x => x.StartDate);

            var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token);

            return result;
        }
        public async Task<List<Offer>> GetAllOfferAsync(CancellationToken token, OfferCategory? category, OfferStatus? status, bool Deleted, int page, int pageSize)
        {
            if (Deleted)
            {
                var query = _context.Set<Offer>().AsNoTracking().IgnoreQueryFilters();

                if (category.HasValue)
                    query = query.Where(x => x.Category == category.Value);

                if (status.HasValue)
                    query = query.Where(x => x.Status == status.Value);

                query = query.OrderByDescending(x => x.StartDate);

                var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token);

                return result;
            }
            else
            {
                var result = await GetActiveOfferAsync(token, category, status, page, pageSize);
                return result;
            }

        }

        public async Task<List<Offer>> GetDeletedOfferAsync(CancellationToken token, OfferCategory? category, OfferStatus? status, int page, int pageSize)
        {
            var query = _dbSet.AsNoTracking().IgnoreQueryFilters().Where(x => x.IsDeleted == true);

            if (category.HasValue)
                query = query.Where(x => x.Category == category.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.OrderByDescending(x => x.DeletedAt);

            var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token);
            return result;
        }

        public async Task<Offer?> GetOfferByIdAsync(CancellationToken token, Guid id)
        {
            //var offer = await base.Get(token, id);

            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, token);
        }
        public async Task<Offer?> GetOfferIncludingDeletedAsync(CancellationToken token, Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, token);
        }
        public async Task<Offer?> GetOfferForUpdateByIdAsync(CancellationToken token, Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(o => o.Id == id, token); // AsNoTracking
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await base.SaveChanges(token);
        }

        public async Task UpdateOfferAsync(CancellationToken token, Offer offer)
        {
            await base.Update(token, offer);
        }
        public async Task DeleteOfferAsync(CancellationToken token, Offer offer)
        {
            await base.Remove(token, offer);
        }

        public async Task DeleteOfferAsync(CancellationToken token, Guid offerId)
        {
            await base.Remove(token, offerId);
        }

        public async Task<List<Offer>> GetExpiredActiveAsync(CancellationToken token)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(o => (o.Status == OfferStatus.Approved || o.Status == OfferStatus.Pending) && o.EndDate < now)
                .ToListAsync(token);
        }

    }
}
