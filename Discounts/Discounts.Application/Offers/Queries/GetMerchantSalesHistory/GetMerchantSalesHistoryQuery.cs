using MediatR;

namespace Discounts.Application.Offers.Queries.GetMerchantSalesHistory;

public record GetMerchantSalesHistoryQuery(int Page = 1, int PageSize = 20) : IRequest<List<MerchantSalesHistoryDto>>
{
}
