using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using MediatR;

namespace Discounts.Application.Offers.Queries.GetMerchantSalesHistory;

public class GetMerchantSalesHistoryHandler : IRequestHandler<GetMerchantSalesHistoryQuery, List<MerchantSalesHistoryDto>>
{
    private readonly IOfferRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetMerchantSalesHistoryHandler(IOfferRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<List<MerchantSalesHistoryDto>> Handle(GetMerchantSalesHistoryQuery request, CancellationToken cancellationToken)
    {
        var merchantId = _currentUserService.UserId;
        if (merchantId == null || merchantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Merchant ID cannot be determined.");
        }

        return await _repository.GetMerchantSalesHistoryAsync(cancellationToken, merchantId.Value, request.Page, request.PageSize).ConfigureAwait(false);
    }
}
