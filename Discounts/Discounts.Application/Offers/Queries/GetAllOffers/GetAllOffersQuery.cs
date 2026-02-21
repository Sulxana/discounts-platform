using Discounts.Domain.Offers;
using MediatR;

namespace Discounts.Application.Offers.Queries.GetAllOffers
{
    public class GetAllOffersQuery : IRequest<List<OfferListItemDto>>
    {
        public GetAllOffersQuery(string? categoryName, OfferStatus? status, bool deleted, int page, int pageSize)
        {
            CategoryName = categoryName;
            Status = status;
            Deleted = deleted;
            Page = page;
            PageSize = pageSize;
        }

        public string? CategoryName { get; set; }
        public OfferStatus? Status { get; set; }
        public bool Deleted { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
