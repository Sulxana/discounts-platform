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


        public void UpdateOfferFields(string title,string description,string? imageUrl,decimal discountedPrice,DateTime endDate)
        {
            Title = title;
            Description = description;
            ImageUrl = imageUrl;
            DiscountedPrice = discountedPrice;
            EndDate = endDate;
        }

    }
}
