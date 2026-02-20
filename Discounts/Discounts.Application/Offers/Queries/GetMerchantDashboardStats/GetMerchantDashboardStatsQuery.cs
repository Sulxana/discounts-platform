using MediatR;

namespace Discounts.Application.Offers.Queries.GetMerchantDashboardStats;

public record GetMerchantDashboardStatsQuery : IRequest<MerchantDashboardStatsDto>
{
}
