namespace Discounts.Application.Offers.Queries.GetOfferById
{
    public class GetOfferByIdQuery
    {
        public Guid Id { get; }
        public GetOfferByIdQuery(Guid id)
        {
            Id = id;
        }
    }

}
