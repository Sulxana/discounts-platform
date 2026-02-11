namespace Discounts.Api.DTO.Offers
{
    public class UpdateOfferRequestDto
    {
        public string? Title { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? ImageUrl { get; set; }

        public decimal? DiscountedPrice { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
