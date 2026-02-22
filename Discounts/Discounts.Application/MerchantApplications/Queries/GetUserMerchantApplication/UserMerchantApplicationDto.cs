namespace Discounts.Application.MerchantApplications.Queries.GetUserMerchantApplication
{
    public class UserMerchantApplicationDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
