using FluentValidation;

namespace Discounts.Application.Offers.Queries.GetActiveOffers
{
    public class GetActiveOffersValidator : AbstractValidator<GetActiveOffersQuery>
    {
        public GetActiveOffersValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }
}
