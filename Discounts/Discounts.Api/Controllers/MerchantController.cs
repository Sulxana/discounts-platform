using Discounts.Application.MerchantApplications.Commands.ApplyMerchant;
using Discounts.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly ApplyMerchantHandler _applyHandler;

        public MerchantController(ApplyMerchantHandler applyHandler)
        {
            _applyHandler = applyHandler;
        }

        [HttpPost("apply")]
        [Authorize]
        public async Task<ActionResult<Guid>> Apply(CancellationToken token)
        {
            var command = new ApplyMerchantCommand();
            var result = await _applyHandler.Handle(command, token);
            return Ok(result);
        }
    }
}
