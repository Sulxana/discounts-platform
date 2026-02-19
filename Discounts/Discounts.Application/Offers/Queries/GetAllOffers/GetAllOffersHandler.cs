using Discounts.Application.Offers.Interfaces;
using FluentValidation;
using Mapster;

namespace Discounts.Application.Offers.Queries.GetAllOffers
{
    public class GetAllOffersHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<GetAllOffersQuery> _validator;

        public GetAllOffersHandler(IOfferRepository repository, IValidator<GetAllOffersQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<OfferListItemDto>> GetAllOffers(CancellationToken token, GetAllOffersQuery query)
        {
            await _validator.ValidateAndThrowAsync(query, token);

            var offers = await _repository.GetAllOfferAsync(token, query.CategoryName, query.Status, query.Deleted, query.Page, query.PageSize);

            return offers.Adapt<List<OfferListItemDto>>();
        }
    }
}
