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

        public async Task AddAsync(CancellationToken token, Offer createOffer)
        {
            await base.Add(token, createOffer);
        }

        public async Task<List<Offer>> GetAllOfferAsync(CancellationToken token, OfferCategory? category, OfferStatus? status, int page, int pageSize)
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

        public async Task<Offer?> GetOfferAsync(CancellationToken token, Guid id)
        {
            var offer = await base.Get(token, id);
            return offer;
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await base.SaveChanges(token);
        }

        public async Task UpdateAsync(CancellationToken token, Offer offer)
        {
            await base.Update(token, offer);
        }
    }
}
