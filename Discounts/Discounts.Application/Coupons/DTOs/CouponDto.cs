namespace Discounts.Application.Coupons.DTOs
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public Guid OfferId { get; set; }
        public Guid? SourceReservationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime PurchasedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRedeemed { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
