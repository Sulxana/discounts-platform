using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Domain.Offers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminModerationController : Controller
    {
        private readonly ISender _mediator;

        public AdminModerationController(ISender mediator)
        {
            _mediator = mediator;
        }

        // --- Offers Moderation ---

        public async Task<IActionResult> Index(int page = 1, CancellationToken token = default)
        {
            var query = new GetAllOffersQuery(null, OfferStatus.Pending, false, page, 20);
            var offers = await _mediator.Send(query, token).ConfigureAwait(false);
            ViewBag.CurrentPage = page;
            return View(offers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOffer(Guid id, CancellationToken token)
        {
            try
            {
                await _mediator.Send(new ApproveOfferCommand(id), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = "Offer approved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectOffer(Guid id, string reason, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "A rejection reason is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _mediator.Send(new RejectOfferCommand(id, reason), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = "Offer rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // Removed Merchant Applications Moderation per user request
    }
}
