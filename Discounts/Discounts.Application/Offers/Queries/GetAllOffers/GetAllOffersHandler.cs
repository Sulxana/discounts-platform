using Discounts.Application.Offers.Interfaces;
using FluentValidation;
using Mapster;
using MediatR;

namespace Discounts.Application.Offers.Queries.GetAllOffers
{
    public class GetAllOffersHandler : IRequestHandler<GetAllOffersQuery, List<OfferListItemDto>>
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<GetAllOffersQuery> _validator;

        public GetAllOffersHandler(IOfferRepository repository, IValidator<GetAllOffersQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<OfferListItemDto>> Handle(GetAllOffersQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

            var offers = await _repository.GetAllOfferAsync(cancellationToken, request.CategoryName, request.Status, request.Deleted, request.Page, request.PageSize).ConfigureAwait(false);

            return offers.Adapt<List<OfferListItemDto>>();
        }
    }
}
