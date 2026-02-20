using Discounts.Application.Coupons.Interfaces;
using Discounts.Domain.Coupons;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Repositories
{
    public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(DiscountsDbContext context) : base(context)
        {
        }

        public async Task AddRangeAsync(CancellationToken token, IEnumerable<Coupon> coupons)
        {
            await _dbSet.AddRangeAsync(coupons, token);
        }

        public async Task<List<Coupon>> GetByUserIdAsync(CancellationToken token, Guid userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.PurchasedAt)
                .ToListAsync(token);
        }

        public async Task<bool> HasCouponsForOfferAsync(Guid offerId, CancellationToken token)
        {
            return await _dbSet.AnyAsync(x => x.OfferId == offerId, token);
        }

        public async Task<Coupon?> GetByIdAsync(Guid id, CancellationToken token)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken token)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Code == code, token);
        }
    }
}
