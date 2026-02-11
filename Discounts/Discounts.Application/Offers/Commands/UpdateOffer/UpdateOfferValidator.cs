using FluentValidation;

namespace Discounts.Application.Offers.Commands.UpdateOffer
{
    public class UpdateOfferValidator : AbstractValidator<UpdateOfferCommand>
    {
        public UpdateOfferValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Title)
                .MinimumLength(3)
                .MaximumLength(50)
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .MinimumLength(10)
                .When(x => x.Description != null);

            RuleFor(x => x.DiscountedPrice)
                .GreaterThan(0)
                .When(x => x.DiscountedPrice.HasValue);

            RuleFor(x => x.EndDate)
               .Must(d => d > DateTime.UtcNow)
               .When(x => x.EndDate.HasValue)
               .WithMessage("EndDate must be in the future.");
        }
    }
}
