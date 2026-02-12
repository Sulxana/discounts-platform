using Discounts.Domain.Offers;

namespace Discounts.Application.Offers.Queries.GetDeletedOffers
{
    public class GetDeletedOffersQuery
    {
        public GetDeletedOffersQuery(OfferCategory? category, OfferStatus? status, int page, int pageSize)
        {
            Category = category;
            Status = status;
            Page = page;
            PageSize = pageSize;
        }

        public OfferCategory? Category { get; set; }
        public OfferStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
