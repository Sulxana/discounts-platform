using Discounts.Application.Offers.Queries.GetMerchantDashboardStats;
using Discounts.Application.Offers.Queries.GetMerchantSalesHistory;
using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Interfaces
{
    public interface IOfferRepository
    {
        Task AddOfferAsync(CancellationToken token, Offer createOffer);
        Task<Offer?> GetOfferByIdAsync(CancellationToken token, Guid id);
        Task<Offer?> GetOfferForUpdateByIdAsync(CancellationToken token, Guid id);
        Task<Offer?> GetOfferIncludingDeletedAsync(CancellationToken token, Guid id);
        Task<List<Offer>> GetActiveOfferAsync(CancellationToken token, string? categoryName, decimal? minPrice, decimal? maxPrice, string? searchTerm, OfferStatus? status,
                                            int page, int pageSize);
        Task<List<Offer>> GetAllOfferAsync(CancellationToken token, string? categoryName, OfferStatus? status,bool Deleted,
                                            int page, int pageSize);
        Task<List<Offer>> GetDeletedOfferAsync(CancellationToken token, string? categoryName, OfferStatus? status,
                                            int page, int pageSize);
        Task UpdateOfferAsync(CancellationToken token, Offer offer);
        Task DeleteOfferAsync(CancellationToken token, Offer offer);
        Task DeleteOfferAsync(CancellationToken token, Guid offerId);
        Task<List<Offer>> GetExpiredActiveAsync(CancellationToken token);
        Task<MerchantDashboardStatsDto> GetMerchantDashboardStatsAsync(CancellationToken token, Guid merchantId);
        Task<List<MerchantSalesHistoryDto>> GetMerchantSalesHistoryAsync(CancellationToken token, Guid merchantId, int page, int pageSize);
        Task<List<Offer>> GetMerchantOffersAsync(CancellationToken token, Guid merchantId, int page, int pageSize);
        Task SaveChangesAsync(CancellationToken token);

    }
}
