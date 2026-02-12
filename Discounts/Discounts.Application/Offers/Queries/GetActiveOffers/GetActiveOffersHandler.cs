using Discounts.Application.Offers.Interfaces;
using FluentValidation;
using Mapster;

namespace Discounts.Application.Offers.Queries.GetActiveOffers
{
    public class GetActiveOffersHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<GetActiveOffersQuery> _validator;

        public GetActiveOffersHandler(IOfferRepository repository, IValidator<GetActiveOffersQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<OfferListItemDto>> GetActiveOffers(CancellationToken token, GetActiveOffersQuery query)
        {
            await _validator.ValidateAndThrowAsync(query, token);

            var offers = await _repository.GetActiveOfferAsync(token, query.Category, query.Status, query.Page, query.PageSize);

            return offers.Adapt<List<OfferListItemDto>>();
        }
    }
}
