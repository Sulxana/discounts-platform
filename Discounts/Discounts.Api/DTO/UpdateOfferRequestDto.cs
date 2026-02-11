namespace Discounts.Api.DTO
{
    public class UpdateOfferRequestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public decimal? DiscountedPrice { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
