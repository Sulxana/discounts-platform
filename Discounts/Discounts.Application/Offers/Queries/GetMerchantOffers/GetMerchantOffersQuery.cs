namespace Discounts.Application.Offers.Queries.GetMerchantOffers
{
    public class GetMerchantOffersQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public GetMerchantOffersQuery(int page = 1, int pageSize = 20)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
