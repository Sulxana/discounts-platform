using FluentValidation;

namespace Discounts.Application.Offers.Queries.GetAllOffers
{
    public class GetAllOffersValidator : AbstractValidator<GetAllOffersQuery>
    {
        public GetAllOffersValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }
}
