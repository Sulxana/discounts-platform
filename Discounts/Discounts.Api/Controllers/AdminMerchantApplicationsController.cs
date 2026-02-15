using Discounts.Application.Common.Security;
using Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication;
using Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Discounts.Api.Controllers
{
    [Route("api/admin/merchant-applications")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class AdminMerchantApplicationsController : ControllerBase
    {
        private readonly ApproveMerchantApplicationHandler _approveHandler;
        private readonly RejectMerchantApplicationHandler _rejectHandler;

        public AdminMerchantApplicationsController(ApproveMerchantApplicationHandler approveHandler, RejectMerchantApplicationHandler rejectHandler)
        {
            _approveHandler = approveHandler;
            _rejectHandler = rejectHandler;
        }

        [HttpPut("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, CancellationToken token)
        {
            await _approveHandler.Handle(new ApproveMerchantApplicationCommand(id), token);
            return NoContent();
        }

        [HttpPut("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromQuery][Required] string reason, CancellationToken token)
        {
            await _rejectHandler.Handle(new RejectMerchantApplicationCommand(id, reason), token);
            return NoContent();
        }
    }
}
