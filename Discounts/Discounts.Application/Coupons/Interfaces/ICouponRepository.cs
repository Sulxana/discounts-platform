using Discounts.Domain.Coupons;

namespace Discounts.Application.Coupons.Interfaces
{
    public interface ICouponRepository
    {
        Task AddRangeAsync(CancellationToken token, IEnumerable<Coupon> coupons);
        Task<List<Coupon>> GetByUserIdAsync(CancellationToken token, Guid userId);
        Task<bool> HasCouponsForOfferAsync(Guid offerId, CancellationToken token);
    }
}
