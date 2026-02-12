using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Interfaces
{
    public interface IOfferRepository
    {
        Task AddOfferAsync(CancellationToken token, Offer createOffer);
        Task<Offer?> GetOfferAsync(CancellationToken token, Guid id);
        Task<List<Offer>> GetAllOfferAsync(CancellationToken token, OfferCategory? category, OfferStatus? status,
                                            int page, int pageSize);
        Task UpdateOfferAsync(CancellationToken token, Offer offer);
        Task DeleteOfferAsync(CancellationToken token, Offer offer);
        Task DeleteOfferAsync(CancellationToken token, Guid offerId);
        Task SaveChangesAsync(CancellationToken token);

    }
}
