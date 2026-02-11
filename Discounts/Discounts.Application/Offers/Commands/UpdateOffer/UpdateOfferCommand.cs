using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Commands.UpdateOffer
{
    public class UpdateOfferCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? ImageUrl { get; set; }

        public decimal DiscountedPrice { get; set; }

        public DateTime EndDate { get; set; }
    }
}
