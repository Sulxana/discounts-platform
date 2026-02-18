namespace Discounts.Application.Offers.Interfaces
{
    public interface IOfferCleanupService
    {
        Task<bool> ProcessOffer(Guid offerId, CancellationToken token);
    }
}
