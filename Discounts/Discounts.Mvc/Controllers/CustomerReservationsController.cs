using Discounts.Application.Reservations.Commands.CreateReservation;
using Discounts.Application.Reservations.Commands.PurchaseReservation;
using Discounts.Application.Reservations.Commands.CancelReservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerReservationsController : Controller
    {
        private readonly CreateReservationHandler _createReservationHandler;
        private readonly Discounts.Application.Reservations.Queries.GetUserReservations.GetUserReservationsHandler _getUserReservationsHandler;
        private readonly PurchaseReservationHandler _purchaseReservationHandler;
        private readonly CancelReservationHandler _cancelReservationHandler;

        public CustomerReservationsController(
            CreateReservationHandler createReservationHandler, 
            Discounts.Application.Reservations.Queries.GetUserReservations.GetUserReservationsHandler getUserReservationsHandler,
            PurchaseReservationHandler purchaseReservationHandler,
            CancelReservationHandler cancelReservationHandler)
        {
            _createReservationHandler = createReservationHandler;
            _getUserReservationsHandler = getUserReservationsHandler;
            _purchaseReservationHandler = purchaseReservationHandler;
            _cancelReservationHandler = cancelReservationHandler;
        }

        [HttpGet]
        public async Task<IActionResult> MyReservations(CancellationToken token)
        {
            var query = new Discounts.Application.Reservations.Queries.GetUserReservations.GetUserReservationsQuery();
            var reservations = await _getUserReservationsHandler.GetUserReservations(token, query);
            return View(reservations);
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
                return RedirectToAction("MyReservations");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Offers", new { id = offerId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(Guid reservationId, CancellationToken token)
        {
            if (reservationId == Guid.Empty)
                return BadRequest("Invalid Reservation ID");

            try
            {
                await _purchaseReservationHandler.Handle(new PurchaseReservationCommand { ReservationId = reservationId }, token);
                TempData["SuccessMessage"] = "Successfully purchased the reserved coupon!";
                return RedirectToAction("MyCoupons", "Customer");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("MyReservations");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken token)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Reservation ID");

            try
            {
                await _cancelReservationHandler.CancelReservationAsync(token, new CancelReservationCommand(id));
                TempData["SuccessMessage"] = "Reservation cancelled successfully.";
                return RedirectToAction("MyReservations");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("MyReservations");
            }
        }
    }
}
