using Discounts.Application.Common.Interfaces;
using Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication;
using Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication;
using Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications;
using Discounts.Application.Users.Commands.BlockUser;
using Discounts.Application.Users.Commands.UnblockUser;
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

        public AdminUsersController(
            IIdentityService identityService, 
            ISender mediator,
            GetAllMerchantApplicationsHandler getApplicationsHandler,
            ApproveMerchantApplicationHandler approveHandler,
            RejectMerchantApplicationHandler rejectHandler)
        {
            _identityService = identityService;
            _mediator = mediator;
            _getApplicationsHandler = getApplicationsHandler;
            _approveHandler = approveHandler;
            _rejectHandler = rejectHandler;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var users = await _identityService.GetAllUsersAsync();
            var applications = await _getApplicationsHandler.Handle(new GetAllMerchantApplicationsQuery(MerchantApplicationStatus.Pending, 1, 100), token);
            
            var viewModel = new AdminUsersViewModel
            {
                Users = users,
                PendingApplications = applications
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveMerchant(Guid applicationId, CancellationToken token)
        {
            try
            {
                await _approveHandler.Handle(new ApproveMerchantApplicationCommand(applicationId), token);
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
                await _rejectHandler.Handle(new RejectMerchantApplicationCommand(applicationId, reason), token);
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
