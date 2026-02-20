using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using MediatR;

namespace Discounts.Application.Offers.Queries.GetMerchantDashboardStats;

public class GetMerchantDashboardStatsHandler : IRequestHandler<GetMerchantDashboardStatsQuery, MerchantDashboardStatsDto>
{
    private readonly IOfferRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetMerchantDashboardStatsHandler(IOfferRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<MerchantDashboardStatsDto> Handle(GetMerchantDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var merchantId = _currentUserService.UserId;
        if (merchantId == null || merchantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Merchant ID cannot be determined.");
        }

        return await _repository.GetMerchantDashboardStatsAsync(cancellationToken, merchantId.Value);
    }
}
