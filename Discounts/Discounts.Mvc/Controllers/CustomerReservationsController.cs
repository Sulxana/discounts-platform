using Discounts.Application.Reservations.Commands.CreateReservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerReservationsController : Controller
    {
        private readonly CreateReservationHandler _createReservationHandler;

        public CustomerReservationsController(CreateReservationHandler createReservationHandler)
        {
            _createReservationHandler = createReservationHandler;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(Guid offerId, int quantity = 1, CancellationToken token = default)
        {
            if (offerId == Guid.Empty)
                return BadRequest("Invalid Offer ID");

            try
            {
                var command = new CreateReservationCommand
                {
                    OfferId = offerId,
                    Quantity = quantity
                };

                await _createReservationHandler.CreateReservation(token, command);

                TempData["SuccessMessage"] = "Successfully reserved the coupon. Please purchase it before it expires.";
                // Redirect to a reservations list page if it existed, otherwise back to home or coupons
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
