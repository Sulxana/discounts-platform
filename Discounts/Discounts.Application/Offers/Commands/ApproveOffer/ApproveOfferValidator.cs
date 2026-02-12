using FluentValidation;

namespace Discounts.Application.Offers.Commands.ApproveOffer
{
    public class ApproveOfferValidator:AbstractValidator<ApproveOfferCommand>
    {
        public ApproveOfferValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
