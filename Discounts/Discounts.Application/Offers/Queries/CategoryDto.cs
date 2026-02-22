namespace Discounts.Application.Offers.Queries
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } = string.Empty;
    }
}
