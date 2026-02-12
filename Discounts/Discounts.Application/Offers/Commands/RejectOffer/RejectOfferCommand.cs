namespace Discounts.Application.Offers.Commands.RejectOffer
{
    public class RejectOfferCommand
    {
        public Guid Id { get; private set; }
        public string Reason { get; }
        public RejectOfferCommand(Guid id,string reason)
        {
            Id = id;
            Reason = reason;
        }
    }
}
