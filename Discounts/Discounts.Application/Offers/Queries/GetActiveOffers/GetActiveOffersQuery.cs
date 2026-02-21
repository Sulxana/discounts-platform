using Discounts.Domain.Offers;
using MediatR;

namespace Discounts.Application.Offers.Queries.GetActiveOffers
{
    public class GetActiveOffersQuery : IRequest<List<OfferListItemDto>>
    {
        public GetActiveOffersQuery(string? categoryName, decimal? minPrice, decimal? maxPrice, string? searchTerm, OfferStatus? status, int page, int pageSize)
        {
            CategoryName = categoryName;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            SearchTerm = searchTerm;
            Status = status;
            Page = page;
            PageSize = pageSize;
        }

        public string? CategoryName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchTerm { get; set; }
        public OfferStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
