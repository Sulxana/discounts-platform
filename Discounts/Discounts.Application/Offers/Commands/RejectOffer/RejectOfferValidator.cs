using FluentValidation;

namespace Discounts.Application.Offers.Commands.RejectOffer
{
    public class RejectOfferValidator:AbstractValidator<RejectOfferCommand>
    {
        public RejectOfferValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
