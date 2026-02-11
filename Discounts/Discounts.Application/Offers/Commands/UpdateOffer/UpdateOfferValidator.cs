using FluentValidation;

namespace Discounts.Application.Offers.Commands.UpdateOffer
{
    public class UpdateOfferValidator : AbstractValidator<UpdateOfferCommand>
    {
        public UpdateOfferValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Title).MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MinimumLength(10);

            RuleFor(x => x.DiscountedPrice)
                .GreaterThan(0);

        }
    }
}
