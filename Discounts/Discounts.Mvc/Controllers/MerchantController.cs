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
            var stats = await _dashboardStatsHandler.Handle(new GetMerchantDashboardStatsQuery(), token).ConfigureAwait(false);
            return View(stats);
        }

        public async Task<IActionResult> SalesHistory(int page = 1, CancellationToken token = default)
        {
            var query = new GetMerchantSalesHistoryQuery(page, 20); // Default to 20 items per page
            var history = await _salesHistoryHandler.Handle(query, token).ConfigureAwait(false);

            ViewBag.CurrentPage = page;

            return View(history);
        }

        [HttpGet]
        public IActionResult Redeem()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Redeem([FromServices] Discounts.Application.Coupons.Commands.RedeemCoupon.RedeemCouponHandler redeemHandler, string code, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("code", "Coupon code is required.");
                return View();
            }

            try
            {
                var command = new Discounts.Application.Coupons.Commands.RedeemCoupon.RedeemCouponCommand { Code = code };
                await redeemHandler.Handle(command, token).ConfigureAwait(false);

                TempData["SuccessMessage"] = $"Coupon '{code}' has been successfully redeemed!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }
    }
}
