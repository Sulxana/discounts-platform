namespace Discounts.Application.Offers.Commands.ApproveOffer
{
    public class ApproveOfferCommand
    {
        public Guid Id { get; private set; }

        public ApproveOfferCommand(Guid id)
        {
            Id = id;
        }
    }
}
