using Discounts.Domain.Categories;
using Discounts.Domain.Reservations;

namespace Discounts.Domain.Offers
{
    public class Offer
    {
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        private Offer() { }

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public Offer(string title, string description, Guid categoryId, string? imageUrl, decimal originalPrice, decimal discountedPrice, int totalCoupons, DateTime startDate, DateTime endDate, Guid merchantId)
        {
            Id = Guid.NewGuid(); // Id is initialized here as per the instruction's constructor body
            Title = title;
            Description = description;
            CategoryId = categoryId; // Changed from Category = category
            ImageUrl = imageUrl;
            OriginalPrice = originalPrice;
            DiscountedPrice = discountedPrice;
            TotalCoupons = totalCoupons;
            RemainingCoupons = totalCoupons;
            StartDate = startDate;
            EndDate = endDate;
            CreatedAt = DateTime.UtcNow; // Reordered as per instruction
            Status = OfferStatus.Pending; // Reordered as per instruction
            MerchantId = merchantId; // Reordered as per instruction
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid MerchantId { get; private set; }
        public void SetMerchantId(Guid merchantId)
        {
            if (MerchantId != Guid.Empty) throw new InvalidOperationException("MerchantId is already set");
            MerchantId = merchantId;
        }
        public required string Title { get; set; } = string.Empty;
        public required string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; private set; } // Added CategoryId
        public Category? Category { get; private set; } // Added Category navigation property
        public string? ImageUrl { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public decimal DiscountedPrice { get; private set; }
        public int TotalCoupons { get; private set; }
        public int RemainingCoupons { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public OfferStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public string? RejectionMessage { get; private set; }
        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        public int MinutesRemaining => (int)Math.Max(0, (EndDate - DateTime.UtcNow).TotalMinutes);

        public virtual ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();

        public void MarkAsDeleted(string? reason = null)
        {
            if (IsDeleted)
                return;
            IsDeleted = true;
            Status = OfferStatus.Deleted;
            DeletedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                RejectionMessage = reason;
            }
        }
        public void UnMarkAsDeleted()
        {
            if (!IsDeleted)
                return;
            IsDeleted = false;
            DeletedAt = null;
            Status = OfferStatus.Pending;
        }

        public void UpdateOfferFields(string? title, string? description, string? imageUrl, decimal? discountedPrice, DateTime? endDate)
        {
            if (title is not null)
                Title = title;

            if (description is not null)
                Description = description;

            if (imageUrl is not null)
                ImageUrl = imageUrl;

            if (discountedPrice.HasValue)
                DiscountedPrice = discountedPrice.Value;

            if (endDate.HasValue)
                EndDate = endDate.Value;
        }

        public void Approve()
        {
            if (IsDeleted || Status != OfferStatus.Pending)
                throw new InvalidOperationException("Only pending offers can be approved.");

            Status = OfferStatus.Approved;
        }
        public void Reject(string message)
        {
            if (IsDeleted || Status != OfferStatus.Pending)
                throw new InvalidOperationException("Only pending offers can be approved.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Rejection reason is required.");
            Status = OfferStatus.Rejected;
            RejectionMessage = message;
        }
        public void Expire()
        {
            if (IsDeleted || (Status != OfferStatus.Approved && Status != OfferStatus.Pending))
                throw new InvalidOperationException("Only pending or approved offers can expire.");
            Status = OfferStatus.Expired;
        }

        public void DecreaseStock(int num = 1)
        {
            if (IsDeleted || Status != OfferStatus.Approved)
                throw new InvalidOperationException("Offer is deleted or not approved");

            if (num < 1)
                throw new ArgumentException("Entered quantity must be positive number.");

            if (RemainingCoupons < num)
                throw new ArgumentOutOfRangeException(nameof(num), "There are not enough coupons");

            RemainingCoupons -= num;
        }

        public void IncreaseStock(int num = 1)
        {
            if (num < 1)
                throw new ArgumentException("Entered quantity must be positive number.");

            RemainingCoupons = Math.Min(TotalCoupons, RemainingCoupons + num);
        }
    }
}
