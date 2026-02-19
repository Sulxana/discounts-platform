using FluentValidation;

namespace Discounts.Application.Coupons.Commands.DirectPurchase
{
    public class DirectPurchaseValidator : AbstractValidator<DirectPurchaseCommand>
    {
        public DirectPurchaseValidator()
        {
            RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required");

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1")
                .LessThanOrEqualTo(10).WithMessage("Cannot purchase more than 10 coupons at once");
        }
    }
}
