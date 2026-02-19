using Discounts.Application.Offers.Interfaces;
using FluentValidation;
using Mapster;

namespace Discounts.Application.Offers.Queries.GetDeletedOffers
{
    public class GetDeletedOffersHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<GetDeletedOffersQuery> _validator;

        public GetDeletedOffersHandler(IOfferRepository repository, IValidator<GetDeletedOffersQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<OfferListItemDto>> GetDeletedOffers(CancellationToken token, GetDeletedOffersQuery query)
        {
            var result = await _repository.GetDeletedOfferAsync(token, query.CategoryName, query.Status, query.Page, query.PageSize);
            return result.Adapt<List<OfferListItemDto>>();
        }
    }
}
