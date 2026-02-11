using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using Mapster;

namespace Discounts.Application.Offers.Queries.GetOfferById
{
    public class GetOfferByIdHandler
    {
        private readonly IOfferRepository _repository;

        public GetOfferByIdHandler(IOfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<OfferDetailsDto> GetOfferById(CancellationToken token, GetOfferByIdQuery query)
        {
            if (query.Id == Guid.Empty)
            {
                throw new ValidationsException("Offer id cannot be empty.");
            }
            var result = await _repository.GetOfferAsync(token, query.Id);
            if (result == null)
                throw new NotFoundException(nameof(Offer), query.Id);

            return result.Adapt<OfferDetailsDto>();
        }
    }
}
