using Discounts.Application.Coupons.Commands.DirectPurchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerCouponsController : Controller
    {
        private readonly DirectPurchaseHandler _directPurchaseHandler;

        public CustomerCouponsController(DirectPurchaseHandler directPurchaseHandler)
        {
            _directPurchaseHandler = directPurchaseHandler;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyDirect(Guid offerId, int quantity = 1, CancellationToken token = default)
        {
            if (offerId == Guid.Empty)
                return BadRequest("Invalid Offer ID");

            try
            {
                var command = new DirectPurchaseCommand
                {
                    OfferId = offerId,
                    Quantity = quantity
                };

                await _directPurchaseHandler.Handle(command, token);

                TempData["SuccessMessage"] = "Successfully purchased the coupon!";
                return RedirectToAction("MyCoupons", "Customer");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Offers", new { id = offerId });
            }
        }
    }
}
