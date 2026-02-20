namespace Discounts.Domain.Coupons
{
    public class Coupon
    {
        private Coupon() { }

        public Coupon(Guid userId, Guid offerId, Guid? reservationId, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OfferId = offerId;
            SourceReservationId = reservationId;
            PurchasedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            Code = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(); // Simple code generation
            IsRedeemed = false;
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid OfferId { get; private set; }
        public Guid? SourceReservationId { get; private set; } // Link to original reservation
        public string Code { get; private set; } // Unique QR/Barcode string
        public DateTime PurchasedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRedeemed { get; private set; }
        public DateTime? RedeemedAt { get; private set; }

        public bool IsActive()
        {
            return !IsRedeemed && DateTime.UtcNow <= ExpiresAt;
        }

        public void Redeem()
        {
            if (IsRedeemed)
                throw new InvalidOperationException("Coupon is already redeemed.");

            if (!IsActive())
                throw new InvalidOperationException("Coupon is expired and cannot be redeemed.");

            IsRedeemed = true;
            RedeemedAt = DateTime.UtcNow;
        }
    }
}
