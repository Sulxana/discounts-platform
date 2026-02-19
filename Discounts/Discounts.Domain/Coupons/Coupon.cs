namespace Discounts.Domain.Coupons
{
    public class Coupon
    {
        private Coupon() { }

        public Coupon(Guid userId, Guid offerId, Guid? reservationId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OfferId = offerId;
            SourceReservationId = reservationId;
            PurchasedAt = DateTime.UtcNow;
            Code = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(); // Simple code generation
            IsRedeemed = false;
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid OfferId { get; private set; }
        public Guid? SourceReservationId { get; private set; } // Link to original reservation
        public string Code { get; private set; } // Unique QR/Barcode string
        public DateTime PurchasedAt { get; private set; }
        public bool IsRedeemed { get; private set; }
        public DateTime? RedeemedAt { get; private set; }

        public void Redeem()
        {
            if (IsRedeemed)
                throw new InvalidOperationException("Coupon is already redeemed.");

            IsRedeemed = true;
            RedeemedAt = DateTime.UtcNow;
        }
    }
}
