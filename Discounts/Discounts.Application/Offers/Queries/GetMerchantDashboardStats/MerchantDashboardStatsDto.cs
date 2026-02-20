namespace Discounts.Application.Offers.Queries.GetMerchantDashboardStats;

public class MerchantDashboardStatsDto
{
    public int TotalOffers { get; set; }
    public int ActiveOffers { get; set; }
    public int ExpiredOffers { get; set; }
    public int PendingOffers { get; set; }
    public int RejectedOffers { get; set; }
}
