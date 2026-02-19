namespace Discounts.Application.Coupons.Commands.DirectPurchase
{
    public class DirectPurchaseCommand
    {
        public Guid OfferId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
