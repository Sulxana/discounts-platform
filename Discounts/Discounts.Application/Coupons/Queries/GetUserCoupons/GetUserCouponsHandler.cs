using Discounts.Application.Common.Interfaces;
using Discounts.Application.Coupons.DTOs;
using Discounts.Application.Coupons.Interfaces;

namespace Discounts.Application.Coupons.Queries.GetUserCoupons
{
    public class GetUserCouponsHandler
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserCouponsHandler(ICouponRepository couponRepository, ICurrentUserService currentUserService)
        {
            _couponRepository = couponRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<CouponDto>> Handle(GetUserCouponsQuery query, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You must be logged in to view your coupons.");
            }

            var coupons = await _couponRepository.GetByUserIdAsync(token, userId.Value).ConfigureAwait(false);

            var dtos = coupons.Select(c => new CouponDto
            {
                Id = c.Id,
                OfferId = c.OfferId,
                SourceReservationId = c.SourceReservationId,
                Code = c.Code,
                PurchasedAt = c.PurchasedAt,
                ExpiresAt = c.ExpiresAt,
                IsRedeemed = c.IsRedeemed,
                RedeemedAt = c.RedeemedAt,
                IsActive = c.IsActive()
            }).ToList();

            return dtos;
        }
    }
}
