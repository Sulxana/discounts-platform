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
        private readonly Discounts.Application.MerchantApplications.Queries.GetUserMerchantApplication.GetUserMerchantApplicationHandler _getUserAppHandler;

        public CustomerController(
            GetUserCouponsHandler getUserCouponsHandler,
            ApplyMerchantHandler applyMerchantHandler,
            Discounts.Application.MerchantApplications.Queries.GetUserMerchantApplication.GetUserMerchantApplicationHandler getUserAppHandler)
        {
            _getUserCouponsHandler = getUserCouponsHandler;
            _applyMerchantHandler = applyMerchantHandler;
            _getUserAppHandler = getUserAppHandler;
        }

        public async Task<IActionResult> MyCoupons(CancellationToken token)
        {
            var appQuery = new Discounts.Application.MerchantApplications.Queries.GetUserMerchantApplication.GetUserMerchantApplicationQuery();
            ViewBag.MerchantApplication = await _getUserAppHandler.Handle(appQuery, token).ConfigureAwait(false);

            var query = new GetUserCouponsQuery();
            var coupons = await _getUserCouponsHandler.Handle(query, token).ConfigureAwait(false);
            return View(coupons);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyForMerchant(CancellationToken token)
        {
            try
            {
                var command = new ApplyMerchantCommand();
                await _applyMerchantHandler.Handle(command, token).ConfigureAwait(false);
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
