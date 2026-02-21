using Discounts.Application.Offers.Queries.GetMerchantDashboardStats;
using Discounts.Application.Offers.Queries.GetMerchantSalesHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Merchant")]
    public class MerchantController : Controller
    {
        private readonly GetMerchantDashboardStatsHandler _dashboardStatsHandler;
        private readonly GetMerchantSalesHistoryHandler _salesHistoryHandler;

        public MerchantController(GetMerchantDashboardStatsHandler dashboardStatsHandler, GetMerchantSalesHistoryHandler salesHistoryHandler)
        {
            _dashboardStatsHandler = dashboardStatsHandler;
            _salesHistoryHandler = salesHistoryHandler;
        }

        public async Task<IActionResult> Dashboard(CancellationToken token)
        {
            var stats = await _dashboardStatsHandler.Handle(new GetMerchantDashboardStatsQuery(), token);
            return View(stats);
        }

        public async Task<IActionResult> SalesHistory(int page = 1, CancellationToken token = default)
        {
            var query = new GetMerchantSalesHistoryQuery(page, 20); // Default to 20 items per page
            var history = await _salesHistoryHandler.Handle(query, token);
            
            // To pass current page to the view for simple pagination checks mapping
            ViewBag.CurrentPage = page;

            return View(history);
        }
    }
}
