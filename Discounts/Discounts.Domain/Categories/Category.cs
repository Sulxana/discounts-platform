using Discounts.Domain.Offers;

namespace Discounts.Domain.Categories
{
    public class Category
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public Category(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            Offers = new List<Offer>();
        }

        // For EF Core
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        private Category() { }
    }
}
