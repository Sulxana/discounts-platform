namespace Discounts.Domain.Offers
{
    public class Offer
    {
        private Offer() { }

        public Offer(string title, string description, OfferCategory category, string? imageUrl, decimal originalPrice, decimal discountedPrice, int totalCoupons, DateTime startDate, DateTime endDate)
        {
            Title = title;
            Description = description;
            Category = category;
            ImageUrl = imageUrl;
            OriginalPrice = originalPrice;
            DiscountedPrice = discountedPrice;
            TotalCoupons = totalCoupons;
            RemainingCoupons = totalCoupons;
            StartDate = startDate;
            EndDate = endDate;
            Status = OfferStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; private set; }
        public string Description { get; private set; }
        public OfferCategory Category { get; private set; }
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

        public virtual ICollection<Discounts.Domain.Reservations.Reservation> Reservations { get; private set; } = new List<Discounts.Domain.Reservations.Reservation>();

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
            if (IsDeleted || Status != OfferStatus.Approved)
                throw new InvalidOperationException("Only pending offers can expire.");
            Status = OfferStatus.Expired;
        }

        public void DecreaseStock(int num = 1)
        {
            if (IsDeleted || Status != OfferStatus.Approved)
                throw new InvalidOperationException("Offer is deleted or not approved");

            if (num < 1)
                    throw new ArgumentException("Entered quantity must be positive number.");

            if (RemainingCoupons < num)
                throw new ArgumentOutOfRangeException("There are not enough coupons");

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
