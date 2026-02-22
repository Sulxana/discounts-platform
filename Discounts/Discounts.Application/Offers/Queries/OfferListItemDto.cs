using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Queries
{
    public class OfferListItemDto
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Category { get; init; }
        public required decimal OriginalPrice { get; init; }
        public required decimal DiscountedPrice { get; init; }
        public required int RemainingCoupons { get; init; }
        public required DateTime EndDate { get; init; }
        public required OfferStatus Status { get; init; }
        public string? RejectionMessage { get; init; }
        public string MerchantName { get; init; } = string.Empty;
        public string? ImageUrl { get; set; }

    }
}
