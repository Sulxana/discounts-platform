using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication;
using Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication;
using Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications;
using Discounts.Application.Users.Commands.BlockUser;
using Discounts.Application.Users.Commands.UnblockUser;
using Discounts.Application.Users.Commands.UpdateUserRole;
using Discounts.Domain.MerchantApplications;
using Discounts.Mvc.Models;
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
        private readonly GetAllMerchantApplicationsHandler _getApplicationsHandler;
        private readonly ApproveMerchantApplicationHandler _approveHandler;
        private readonly RejectMerchantApplicationHandler _rejectHandler;
        private readonly string _superAdminEmail;

        public AdminUsersController(
            IIdentityService identityService,
            ISender mediator,
            GetAllMerchantApplicationsHandler getApplicationsHandler,
            ApproveMerchantApplicationHandler approveHandler,
            RejectMerchantApplicationHandler rejectHandler,
            IConfiguration configuration)
        {
            _identityService = identityService;
            _mediator = mediator;
            _getApplicationsHandler = getApplicationsHandler;
            _approveHandler = approveHandler;
            _rejectHandler = rejectHandler;
            _superAdminEmail = configuration["SuperAdminEmail"] ?? string.Empty;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var users = await _identityService.GetAllUsersAsync().ConfigureAwait(false);
            var applications = await _getApplicationsHandler.Handle(new GetAllMerchantApplicationsQuery(MerchantApplicationStatus.Pending, 1, 100), token).ConfigureAwait(false);

            var viewModel = new AdminUsersViewModel
            {
                Users = users,
                PendingApplications = applications,
                SuperAdminEmail = _superAdminEmail,
                AvailableRoles = new List<string>
                {
                    Roles.Administrator,
                    Roles.Merchant,
                    Roles.Customer
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlock(Guid id, bool isCurrentlyBlocked, CancellationToken token)
        {
            try
            {
                if (isCurrentlyBlocked)
                {
                    await _mediator.Send(new UnblockUserCommand(id), token).ConfigureAwait(false);
                    TempData["SuccessMessage"] = "User successfully unblocked.";
                }
                else
                {
                    await _mediator.Send(new BlockUserCommand(id), token).ConfigureAwait(false);
                    TempData["SuccessMessage"] = "User successfully blocked.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(Guid id, string newRole, CancellationToken token)
        {
            var currentUserEmail = User.Identity?.Name;
            var (_, _, userEmail, _) = await _identityService.GetUserByIdAsync(id).ConfigureAwait(false);

            // Prevent self-role change
            if (string.Equals(currentUserEmail, userEmail, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "You cannot change your own role.";
                return RedirectToAction(nameof(Index));
            }

            // Protect the super-admin account from any role change
            if (string.Equals(_superAdminEmail, userEmail, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "The super-admin's role cannot be changed.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _mediator.Send(new UpdateUserRoleCommand { UserId = id, NewRole = newRole }, token).ConfigureAwait(false);
                TempData["SuccessMessage"] = $"Role updated to '{newRole}' successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveMerchant(Guid applicationId, CancellationToken token)
        {
            try
            {
                await _approveHandler.Handle(new ApproveMerchantApplicationCommand(applicationId), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = "User successfully approved as Merchant!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectMerchant(Guid applicationId, string reason, CancellationToken token)
        {
            try
            {
                await _rejectHandler.Handle(new RejectMerchantApplicationCommand(applicationId, reason), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = "Merchant application rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
