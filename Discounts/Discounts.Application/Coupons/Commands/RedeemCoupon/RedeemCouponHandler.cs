using Discounts.Application.Common.Interfaces;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Application.Offers.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Discounts.Application.Coupons.Commands.RedeemCoupon
{
    public class RedeemCouponHandler
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RedeemCouponHandler> _logger;
        private readonly IValidator<RedeemCouponCommand> _validator;

        public RedeemCouponHandler(
            ICouponRepository couponRepository,
            IOfferRepository offerRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ILogger<RedeemCouponHandler> logger,
            IValidator<RedeemCouponCommand> validator)
        {
            _couponRepository = couponRepository;
            _offerRepository = offerRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _validator = validator;
        }

        public async Task Handle(RedeemCouponCommand command, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(command, token).ConfigureAwait(false);

            var merchantId = _currentUserService.UserId;
            if (merchantId == null)
                throw new UnauthorizedAccessException("You must be logged in as a merchant to redeem coupons.");

            var coupon = await _couponRepository.GetByCodeAsync(command.Code, token).ConfigureAwait(false);
            if (coupon == null)
                throw new KeyNotFoundException($"Coupon with code {command.Code} not found.");

            var offer = await _offerRepository.GetOfferByIdAsync(token, coupon.OfferId).ConfigureAwait(false);
            if (offer == null)
                throw new KeyNotFoundException($"Associated Offer for coupon {command.Code} not found.");

            if (offer.MerchantId != merchantId.Value)
                throw new UnauthorizedAccessException("You can only redeem coupons for your own offers.");

            coupon.Redeem();

            await _unitOfWork.SaveChangesAsync(token).ConfigureAwait(false);

            _logger.LogInformation($"Merchant {merchantId} successfully redeemed coupon {coupon.Code}.");
        }
    }
}
