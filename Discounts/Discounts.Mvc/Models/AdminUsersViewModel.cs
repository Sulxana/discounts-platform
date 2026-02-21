using Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications;

namespace Discounts.Mvc.Models
{
    public class AdminUsersViewModel
    {
        public List<(Guid Id, string Email, string FirstName, string LastName, bool IsBlocked, IList<string> Roles)> Users { get; set; } = new();
        public List<MerchantApplicationDto> PendingApplications { get; set; } = new();
    }
}
