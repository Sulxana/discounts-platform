using FluentValidation;

namespace Discounts.Application.Offers.Commands.UpdateOffer
{
    public class UpdateOfferValidator : AbstractValidator<UpdateOfferCommand>
    {
        public UpdateOfferValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Offer Id must not be empty.");

            RuleFor(x => x.Title)
                .MinimumLength(3)
                .WithMessage("Title must be at least 3 characters long.")
                .MaximumLength(50)
                .WithMessage("Title must not exceed 50 characters.")
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .MinimumLength(10)
                .WithMessage("Description must be at least 10 characters long.")
                .When(x => x.Description != null);

            RuleFor(x => x.DiscountedPrice)
                .GreaterThan(0)
                .WithMessage("Discounted price must be greater than 0.")
                .When(x => x.DiscountedPrice.HasValue);

            RuleFor(x => x.EndDate)
                .Must(d => d > DateTime.UtcNow)
                .WithMessage("End date must be in the future.")
                .When(x => x.EndDate.HasValue);

        }
    }
}
