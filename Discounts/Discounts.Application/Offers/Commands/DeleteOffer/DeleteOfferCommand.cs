namespace Discounts.Application.Offers.Commands.DeleteOffer
{
    public class DeleteOfferCommand
    {
        public Guid Id { get; set; }

        public DeleteOfferCommand(Guid id)
        {
            Id = id;
        }
    }
}
