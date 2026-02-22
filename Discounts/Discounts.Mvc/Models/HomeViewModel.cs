using Discounts.Application.Offers.Queries;

namespace Discounts.Mvc.Models
{
    public class HomeViewModel
    {
        public List<OfferListItemDto> Offers { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public bool HasNextPage => Offers.Count == PageSize; 
    }
}
