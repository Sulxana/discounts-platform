using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Commands.CreateOffer
{
    public class CreateOfferCommand
    {
        /// <summary>
        /// Title of the offer
        /// </summary>
        /// <example>Summer Sale 50% Off</example>
        public string Title { get; set; }

        /// <summary>
        /// Description of the offer
        /// </summary>
        /// <example>Get 50% off on all summer clothing items.</example>
        public string Description { get; set; }

        /// <summary>
        /// Category ID
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Optional image URL
        /// </summary>
        /// <example>https://example.com/images/summer-sale.jpg</example>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Original price before discount
        /// </summary>
        /// <example>100.00</example>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Discounted price
        /// </summary>
        /// <example>50.00</example>
        public decimal DiscountedPrice { get; set; }

        /// <summary>
        /// Total number of coupons available
        /// </summary>
        /// <example>100</example>
        public int TotalCoupons { get; set; }

        /// <summary>
        /// Start date of the offer
        /// </summary>
        /// <example>2026-06-01T00:00:00Z</example>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the offer
        /// </summary>
        /// <example>2026-06-30T23:59:59Z</example>
        public DateTime EndDate { get; set; }
    }
}
