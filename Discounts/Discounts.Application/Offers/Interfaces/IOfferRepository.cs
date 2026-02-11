using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Interfaces
{
    public interface IOfferRepository
    {
        Task AddAsync(CancellationToken token, Offer createOffer);
        Task<Offer?> GetOfferAsync(CancellationToken token,Guid id);
        Task UpdateAsync(CancellationToken token, Offer offer);
        Task SaveChangesAsync(CancellationToken token);

    }
}
