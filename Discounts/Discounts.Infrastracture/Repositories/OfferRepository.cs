using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Infrastracture.Persistence.Context;

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

        public async Task<Offer?> GetAsync(CancellationToken token, Guid id)
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
