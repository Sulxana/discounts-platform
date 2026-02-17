namespace Discounts.Application.Offers.Commands.DeleteOffer
{
    public class DeleteOfferCommand
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }

        public DeleteOfferCommand(Guid id, string? reason = null)
        {
            Id = id;
            Reason = reason;
        }
    }
}
