using Discounts.Application.Offers.Queries.GetOfferById;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    public class OffersController : Controller
    {
        private readonly GetOfferByIdHandler _getOfferByIdHandler;

        public OffersController(GetOfferByIdHandler getOfferByIdHandler)
        {
            _getOfferByIdHandler = getOfferByIdHandler;
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken token)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var query = new GetOfferByIdQuery(id);
                var offer = await _getOfferByIdHandler.GetOfferById(token, query);
                return View(offer);
            }
            catch (Exception ex)
            {
                // In a deeper implementation, handle NotFoundException directly
                TempData["ErrorMessage"] = "Offer not found or unavailable.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
