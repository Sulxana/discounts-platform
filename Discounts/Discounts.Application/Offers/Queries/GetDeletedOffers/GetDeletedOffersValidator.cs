using FluentValidation;

namespace Discounts.Application.Offers.Queries.GetDeletedOffers
{
    public class GetDeletedOffersValidator : AbstractValidator<GetDeletedOffersQuery>
    {
        public GetDeletedOffersValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }
}
