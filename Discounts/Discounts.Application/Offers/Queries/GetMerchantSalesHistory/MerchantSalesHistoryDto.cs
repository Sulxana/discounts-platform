namespace Discounts.Application.Offers.Queries.GetMerchantSalesHistory;

public class MerchantSalesHistoryDto
{
    public Guid CouponId { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid OfferId { get; set; }
    public string OfferTitle { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime PurchasedAt { get; set; }
    public bool IsRedeemed { get; set; }
    public DateTime? RedeemedAt { get; set; }
}
