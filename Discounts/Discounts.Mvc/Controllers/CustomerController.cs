using Discounts.Application.Coupons.Queries.GetUserCoupons;
using Discounts.Application.MerchantApplications.Commands.ApplyMerchant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly GetUserCouponsHandler _getUserCouponsHandler;
        private readonly ApplyMerchantHandler _applyMerchantHandler;

        public CustomerController(GetUserCouponsHandler getUserCouponsHandler, ApplyMerchantHandler applyMerchantHandler)
        {
            _getUserCouponsHandler = getUserCouponsHandler;
            _applyMerchantHandler = applyMerchantHandler;
        }

        public async Task<IActionResult> MyCoupons(CancellationToken token)
        {
            var query = new GetUserCouponsQuery();
            var coupons = await _getUserCouponsHandler.Handle(query, token);
            return View(coupons);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyForMerchant(CancellationToken token)
        {
            try
            {
                var command = new ApplyMerchantCommand();
                await _applyMerchantHandler.Handle(command, token);
                TempData["SuccessMessage"] = "Successfully applied to become a merchant. Please wait for an admin to approve your application.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("MyCoupons");
        }
    }
}
