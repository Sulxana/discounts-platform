using FluentValidation;

namespace Discounts.Application.Offers.Commands.CreateOffer
{
    public class CreateOfferValidator : AbstractValidator<CreateOfferCommand>
    {
        public CreateOfferValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 50).WithMessage("Title must be between 3 and 50 characters.");


            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(10);

            RuleFor(x => x.OriginalPrice)
                .GreaterThan(0).WithMessage("Original Price must be greater than 0.");

            RuleFor(x => x.DiscountedPrice)
                .LessThan(x => x.OriginalPrice)
                .GreaterThan(0).WithMessage("Discounted Price must be greater than 0.");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate).WithMessage("startDate must be less than endDate.");

            RuleFor(x => x.TotalCoupons)
                .GreaterThan(0).WithMessage("Total Coupons count must be greater than 0.");

        }
    }
}
