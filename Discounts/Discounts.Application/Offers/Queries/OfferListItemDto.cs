using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Queries
{
    public class OfferListItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public OfferCategory Category { get; init; }
        public decimal OriginalPrice { get; init; }
        public decimal DiscountedPrice { get; init; }
        public int RemainingCoupons { get; init; }
        public DateTime EndDate { get; init; }
        public OfferStatus Status { get; init; }
        public string? RejectionMessage { get; init; }
        
    }
}
