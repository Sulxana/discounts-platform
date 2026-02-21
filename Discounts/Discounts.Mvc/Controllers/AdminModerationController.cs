using Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication;
using Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication;
using Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications;
using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Domain.MerchantApplications;
using Discounts.Domain.Offers;
using Mapster;
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
            var offers = await _mediator.Send(query, token);
            ViewBag.CurrentPage = page;
            return View(offers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOffer(Guid id, CancellationToken token)
        {
            try
            {
                await _mediator.Send(new ApproveOfferCommand(id), token);
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
                await _mediator.Send(new RejectOfferCommand(id, reason), token);
                TempData["SuccessMessage"] = "Offer rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // --- Merchant Applications Moderation ---

        public async Task<IActionResult> MerchantApplications(int page = 1, CancellationToken token = default)
        {
            var query = new GetAllMerchantApplicationsQuery(MerchantApplicationStatus.Pending, page, 20);
            var apps = await _mediator.Send(query, token);
            ViewBag.CurrentPage = page;
            return View(apps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveApplication(Guid id, CancellationToken token)
        {
            try
            {
                await _mediator.Send(new ApproveMerchantApplicationCommand(id), token);
                TempData["SuccessMessage"] = "Merchant application approved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(MerchantApplications));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectApplication(Guid id, string reason, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "A rejection reason is required.";
                return RedirectToAction(nameof(MerchantApplications));
            }

            try
            {
                await _mediator.Send(new RejectMerchantApplicationCommand(id, reason), token);
                TempData["SuccessMessage"] = "Merchant application rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(MerchantApplications));
        }
    }
}
