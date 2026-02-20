using FluentValidation;

namespace Discounts.Application.Coupons.Commands.RedeemCoupon
{
    public class RedeemCouponCommandValidator : AbstractValidator<RedeemCouponCommand>
    {
        public RedeemCouponCommandValidator()
        {
            RuleFor(v => v.Code)
                .NotEmpty().WithMessage("Coupon Code is required.");
        }
    }
}
