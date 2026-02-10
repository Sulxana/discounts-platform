using FluentValidation;

namespace Discounts.Application.Offers.Commands.CreateOffer
{
    public class CreateOfferValidator : AbstractValidator<CreateOfferCommand>
    {
        public CreateOfferValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MinimumLength(10);

            RuleFor(x => x.OriginalPrice)
                .GreaterThan(0);

            RuleFor(x => x.DiscountedPrice)
                .LessThan(x => x.OriginalPrice)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate);

            RuleFor(x => x.TotalCoupons)
                .GreaterThan(0);

        }
    }
}
