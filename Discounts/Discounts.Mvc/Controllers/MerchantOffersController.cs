using Discounts.Application.Categories.Queries.GetAllCategories;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.Queries.GetMerchantOffers;
using Discounts.Application.Offers.Queries.GetOfferById;
using Discounts.Application.Settings.Interfaces;
using Discounts.Domain.Settings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Merchant")]
    public class MerchantOffersController : Controller
    {
        private readonly GetMerchantOffersHandler _getMerchantOffersHandler;
        private readonly CreateOfferHandler _createOfferHandler;
        private readonly UpdateOfferHandler _updateOfferHandler;
        private readonly GetOfferByIdHandler _getOfferByIdHandler;
        private readonly ISender _mediator;
        private readonly IGlobalSettingsService _settingsService;

        public MerchantOffersController(
            GetMerchantOffersHandler getMerchantOffersHandler,
            CreateOfferHandler createOfferHandler,
            UpdateOfferHandler updateOfferHandler,
            GetOfferByIdHandler getOfferByIdHandler,
            ISender mediator,
            IGlobalSettingsService settingsService)
        {
            _getMerchantOffersHandler = getMerchantOffersHandler;
            _createOfferHandler = createOfferHandler;
            _updateOfferHandler = updateOfferHandler;
            _getOfferByIdHandler = getOfferByIdHandler;
            _mediator = mediator;
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index(int page = 1, CancellationToken token = default)
        {
            var query = new GetMerchantOffersQuery(page, 20);
            var offers = await _getMerchantOffersHandler.Handle(query, token);
            ViewBag.CurrentPage = page;
            return View(offers);
        }

        private async Task FetchCategoriesToViewBag(CancellationToken token)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery(), token);
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken token)
        {
            await FetchCategoriesToViewBag(token);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfferCommand command, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                await FetchCategoriesToViewBag(token);
                return View(command);
            }

            try
            {
                await _createOfferHandler.CreateOffer(token, command);
                TempData["SuccessMessage"] = "Offer created successfully. It is now pending approval.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await FetchCategoriesToViewBag(token);
                return View(command);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken token)
        {
            if (id == Guid.Empty) return NotFound();

            try
            {
                var offer = await _getOfferByIdHandler.GetOfferById(token, new GetOfferByIdQuery(id));

                var editWindowHours = await _settingsService.GetIntAsync(
                    SettingKeys.MerchantEditWindowHours, defaultValue: 24, token);
                var cutoffTime = offer.CreatedAt.AddHours(editWindowHours);
                if (DateTime.UtcNow > cutoffTime)
                {
                    TempData["ErrorMessage"] = $"This offer can no longer be edited. The {editWindowHours}-hour edit window has passed.";
                    return RedirectToAction("Index");
                }

                var command = new UpdateOfferCommand
                {
                    Id = offer.Id,
                    Title = offer.Title,
                    Description = offer.Description,
                    ImageUrl = offer.ImageUrl,
                    DiscountedPrice = offer.DiscountedPrice,
                    EndDate = offer.EndDate
                };

                return View(command);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateOfferCommand command, CancellationToken token)
        {
            if (id != command.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(command);
            }

            try
            {
                await _updateOfferHandler.UpdateOfferAsync(token, command);
                TempData["SuccessMessage"] = "Offer updated successfully.";
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(command);
            }
        }
    }
}
