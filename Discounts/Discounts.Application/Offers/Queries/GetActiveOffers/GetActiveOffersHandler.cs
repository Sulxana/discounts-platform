using Discounts.Application.Offers.Interfaces;
using FluentValidation;
using Mapster;
using MediatR;

namespace Discounts.Application.Offers.Queries.GetActiveOffers
{
    public class GetActiveOffersHandler : IRequestHandler<GetActiveOffersQuery, List<OfferListItemDto>>
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<GetActiveOffersQuery> _validator;

        public GetActiveOffersHandler(IOfferRepository repository, IValidator<GetActiveOffersQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<OfferListItemDto>> Handle(GetActiveOffersQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var offers = await _repository.GetActiveOfferAsync(cancellationToken, request.CategoryName, request.MinPrice, request.MaxPrice, request.SearchTerm, request.Status, request.Page, request.PageSize);

            return offers.Adapt<List<OfferListItemDto>>();
        }
    }
}
