using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Infrastracture.Persistence.Context;
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
            await Add(token, createOffer).ConfigureAwait(false);
        }

        public async Task<List<Offer>> GetActiveOfferAsync(CancellationToken token, string? categoryName, decimal? minPrice, decimal? maxPrice, string? searchTerm, OfferStatus? status, int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var query = (IQueryable<Offer>)_context.Set<Offer>().AsNoTracking().Include(x => x.Category)
                .Where(x => x.Status == OfferStatus.Approved && x.EndDate > now);

            if (!string.IsNullOrEmpty(categoryName))
                query = query.Where(x => x.Category!.Name == categoryName);

            if (minPrice.HasValue)
                query = query.Where(x => x.DiscountedPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(x => x.DiscountedPrice <= maxPrice.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm));

            query = query.OrderByDescending(x => x.StartDate);

            var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token).ConfigureAwait(false);

            return result;
        }
        public async Task<List<Offer>> GetAllOfferAsync(CancellationToken token, string? categoryName, OfferStatus? status, bool deleted, int page, int pageSize)
        {
            if (deleted)
            {
                var query = (IQueryable<Offer>)_context.Set<Offer>().AsNoTracking().IgnoreQueryFilters().Include(x => x.Category);

                if (!string.IsNullOrEmpty(categoryName))
                    query = query.Where(x => x.Category!.Name == categoryName);

                if (status.HasValue)
                    query = query.Where(x => x.Status == status.Value);

                query = query.OrderByDescending(x => x.StartDate);

                var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token).ConfigureAwait(false);

                return result;
            }
            else
            {
                var query = (IQueryable<Offer>)_context.Set<Offer>().AsNoTracking().IgnoreQueryFilters().Include(x => x.Category).Where(x => x.IsDeleted == false);

                if (!string.IsNullOrEmpty(categoryName))
                    query = query.Where(x => x.Category!.Name == categoryName);

                if (status.HasValue)
                    query = query.Where(x => x.Status == status.Value);

                query = query.OrderByDescending(x => x.StartDate);

                var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token).ConfigureAwait(false);
                return result;
            }

        }

        public async Task<List<Offer>> GetDeletedOfferAsync(CancellationToken token, string? categoryName, OfferStatus? status, int page, int pageSize)
        {
            var query = (IQueryable<Offer>)_dbSet.AsNoTracking().IgnoreQueryFilters().Include(x => x.Category).Where(x => x.IsDeleted == true);

            if (!string.IsNullOrEmpty(categoryName))
                query = query.Where(x => x.Category!.Name == categoryName);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.OrderByDescending(x => x.DeletedAt);

            var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token).ConfigureAwait(false);
            return result;
        }

        public async Task<Offer?> GetOfferByIdAsync(CancellationToken token, Guid id)
        {
            //var offer = await base.GetByIdAsync(token, id);

            return await _dbSet.AsNoTracking()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(o => o.Id == id, token).ConfigureAwait(false);
        }
        public async Task<Offer?> GetOfferIncludingDeletedAsync(CancellationToken token, Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(o => o.Id == id, token).ConfigureAwait(false);
        }
        public async Task<Offer?> GetOfferForUpdateByIdAsync(CancellationToken token, Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(o => o.Id == id, token).ConfigureAwait(false); // AsNoTracking
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await SaveChanges(token).ConfigureAwait(false);
        }

        public async Task UpdateOfferAsync(CancellationToken token, Offer offer)
        {
            await Update(token, offer).ConfigureAwait(false);
        }
        public async Task DeleteOfferAsync(CancellationToken token, Offer offer)
        {
            await Remove(token, offer).ConfigureAwait(false);
        }

        public async Task DeleteOfferAsync(CancellationToken token, Guid offerId)
        {
            await Remove(token, offerId).ConfigureAwait(false);
        }

        public async Task<List<Offer>> GetExpiredActiveAsync(CancellationToken token)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(o => (o.Status == OfferStatus.Approved || o.Status == OfferStatus.Pending) && o.EndDate < now)
                .ToListAsync(token).ConfigureAwait(false);
        }

        public async Task<Application.Offers.Queries.GetMerchantDashboardStats.MerchantDashboardStatsDto> GetMerchantDashboardStatsAsync(CancellationToken token, Guid merchantId)
        {
            var stats = new Application.Offers.Queries.GetMerchantDashboardStats.MerchantDashboardStatsDto();

            stats.TotalOffers = await _dbSet.Where(o => o.MerchantId == merchantId).CountAsync(token).ConfigureAwait(false);
            stats.ActiveOffers = await _dbSet.Where(o => o.MerchantId == merchantId && o.Status == OfferStatus.Approved).CountAsync(token).ConfigureAwait(false);
            stats.ExpiredOffers = await _dbSet.Where(o => o.MerchantId == merchantId && o.Status == OfferStatus.Expired).CountAsync(token).ConfigureAwait(false);
            stats.PendingOffers = await _dbSet.Where(o => o.MerchantId == merchantId && o.Status == OfferStatus.Pending).CountAsync(token).ConfigureAwait(false);
            stats.RejectedOffers = await _dbSet.Where(o => o.MerchantId == merchantId && o.Status == OfferStatus.Rejected).CountAsync(token).ConfigureAwait(false);

            return stats;
        }

        public async Task<List<Application.Offers.Queries.GetMerchantSalesHistory.MerchantSalesHistoryDto>> GetMerchantSalesHistoryAsync(CancellationToken token, Guid merchantId, int page, int pageSize)
        {
            var query = from coupon in _context.Set<Discounts.Domain.Coupons.Coupon>()
                        join offer in _dbSet on coupon.OfferId equals offer.Id
                        join user in _context.Set<Discounts.Infrastracture.Identity.ApplicationUser>() on coupon.UserId equals user.Id
                        where offer.MerchantId == merchantId
                        orderby coupon.PurchasedAt descending
                        select new Application.Offers.Queries.GetMerchantSalesHistory.MerchantSalesHistoryDto
                        {
                            CouponId = coupon.Id,
                            Code = coupon.Code,
                            OfferId = offer.Id,
                            OfferTitle = offer.Title,
                            CustomerId = user.Id,
                            CustomerName = user.FirstName + " " + user.LastName,
                            PurchasedAt = coupon.PurchasedAt,
                            IsRedeemed = coupon.IsRedeemed,
                            RedeemedAt = coupon.RedeemedAt
                        };

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token).ConfigureAwait(false);
        }

        public async Task<List<Offer>> GetMerchantOffersAsync(CancellationToken token, Guid merchantId, int page, int pageSize)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.MerchantId == merchantId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token).ConfigureAwait(false);
        }

    }
}
