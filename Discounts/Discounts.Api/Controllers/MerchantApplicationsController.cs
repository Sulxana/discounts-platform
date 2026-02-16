using Discounts.Application.Common.Security;
using Discounts.Application.MerchantApplications.Commands.ApplyMerchant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [Route("api/merchant-applications")]
    [ApiController]
    [Authorize(Roles = Roles.Customer)]
    public class MerchantApplicationsController : ControllerBase
    {
        private readonly ApplyMerchantHandler _applyMerchantHandler;

        public MerchantApplicationsController(ApplyMerchantHandler applyMerchantHandler)
        {
            _applyMerchantHandler = applyMerchantHandler;
        }

        [HttpPost()]
        public async Task<ActionResult<Guid>> Apply(CancellationToken token)
        {
            var command = new ApplyMerchantCommand();
            var result = await _applyMerchantHandler.Handle(command, token);
            return Ok(result);
        }
    }
}
