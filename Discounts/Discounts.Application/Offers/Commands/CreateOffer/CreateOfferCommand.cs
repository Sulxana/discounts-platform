using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Commands.CreateOffer
{
    public class CreateOfferCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public OfferCategory Category { get; set; }
        public string? ImageUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int TotalCoupons { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
