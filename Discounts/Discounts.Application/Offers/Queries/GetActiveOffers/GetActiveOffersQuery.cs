using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Queries.GetActiveOffers
{
    public class GetActiveOffersQuery
    {
        public GetActiveOffersQuery(string? categoryName, OfferStatus? status, int page, int pageSize)
        {
            CategoryName = categoryName;
            Status = status;
            Page = page;
            PageSize = pageSize;
        }

        public string? CategoryName { get; set; }
        public OfferStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
