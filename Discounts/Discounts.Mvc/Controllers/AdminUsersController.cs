using Discounts.Application.Common.Interfaces;
using Discounts.Application.Users.Commands.BlockUser;
using Discounts.Application.Users.Commands.UnblockUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminUsersController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly ISender _mediator;

        public AdminUsersController(IIdentityService identityService, ISender mediator)
        {
            _identityService = identityService;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _identityService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlock(Guid id, bool isCurrentlyBlocked, CancellationToken token)
        {
            try
            {
                if (isCurrentlyBlocked)
                {
                    await _mediator.Send(new UnblockUserCommand(id), token);
                    TempData["SuccessMessage"] = "User successfully unblocked.";
                }
                else
                {
                    await _mediator.Send(new BlockUserCommand(id), token);
                    TempData["SuccessMessage"] = "User successfully blocked.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
