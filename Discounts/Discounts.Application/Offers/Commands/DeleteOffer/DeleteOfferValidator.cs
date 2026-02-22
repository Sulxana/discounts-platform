using FluentValidation;

namespace Discounts.Application.Offers.Commands.DeleteOffer
{
    public class DeleteOfferValidator : AbstractValidator<DeleteOfferCommand>
    {
        public DeleteOfferValidator()
        {
            RuleFor(x => x.Id).NotEmpty()
                .WithMessage("Offer Id must not be empty.");
        }
    }
}
