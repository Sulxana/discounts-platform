using Discounts.Application.Settings.Commands.UpdateSetting;
using Discounts.Application.Settings.Queries.GetAllSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminSettingsController : Controller
    {
        private readonly ISender _mediator;

        public AdminSettingsController(ISender mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var settings = await _mediator.Send(new GetAllSettingsQuery(), token).ConfigureAwait(false);
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string key, string value, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                TempData["ErrorMessage"] = "Setting key and value are required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _mediator.Send(new UpdateSettingCommand(key, value), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = $"Setting '{key}' updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
