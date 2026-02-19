using Discounts.Application.Common.Interfaces;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Domain.Coupons;

namespace Discounts.Application.Coupons.Queries.GetMyCoupons
{
    public class GetMyCouponsHandler
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyCouponsHandler(ICouponRepository couponRepository, ICurrentUserService currentUserService)
        {
            _couponRepository = couponRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<Coupon>> Handle(GetMyCouponsQuery request, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
               throw new UnauthorizedAccessException("User is not authenticated.");
            }

            return await _couponRepository.GetByUserIdAsync(token, userId.Value);
        }
    }
}
