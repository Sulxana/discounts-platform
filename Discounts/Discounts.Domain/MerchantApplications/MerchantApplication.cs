namespace Discounts.Domain.MerchantApplications
{
    public class MerchantApplication
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public MerchantApplicationStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? ReviewedAtUtc { get; private set; }
        public string? RejectionReason { get; private set; }

        private MerchantApplication() { } 

        public MerchantApplication(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Status = MerchantApplicationStatus.Pending;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void Approve()
        {
            if (Status != MerchantApplicationStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot approve application in status {Status}");
            }

            Status = MerchantApplicationStatus.Approved;
            ReviewedAtUtc = DateTime.UtcNow;
        }

        public void Reject(string reason)
        {
            if (Status != MerchantApplicationStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot reject application in status {Status}");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("Rejection reason cannot be empty", nameof(reason));
            }

            Status = MerchantApplicationStatus.Rejected;
            RejectionReason = reason;
            ReviewedAtUtc = DateTime.UtcNow;
        }
    }
}
