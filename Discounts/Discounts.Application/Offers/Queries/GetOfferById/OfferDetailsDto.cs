using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Queries.GetOfferById
{
    public class OfferDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CategoryDto Category { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int TotalCoupons { get; set; }
        public int RemainingCoupons { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public OfferStatus Status { get; set; }
    }
}
