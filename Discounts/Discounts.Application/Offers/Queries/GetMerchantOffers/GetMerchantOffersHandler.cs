using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Mapster;

namespace Discounts.Application.Offers.Queries.GetMerchantOffers
{
    public class GetMerchantOffersHandler
    {
        private readonly IOfferRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetMerchantOffersHandler(IOfferRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<List<OfferListItemDto>> Handle(GetMerchantOffersQuery query, CancellationToken token)
        {
            var merchantId = _currentUserService.UserId;
            if (merchantId == null)
            {
                throw new UnauthorizedAccessException("You must be logged in as a merchant.");
            }

            var offers = await _repository.GetMerchantOffersAsync(token, merchantId.Value, query.Page, query.PageSize).ConfigureAwait(false);
            return offers.Adapt<List<OfferListItemDto>>();
        }
    }
}
