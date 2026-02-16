using Discounts.Domain.MerchantApplications;

namespace Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications
{
    public record GetAllMerchantApplicationsQuery(MerchantApplicationStatus? Status, int Page, int PageSize);
}
