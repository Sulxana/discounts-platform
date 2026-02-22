using Asp.Versioning;
using Discounts.Application.Common.Security;
using Discounts.Application.Settings.Commands.UpdateSetting;
using Discounts.Application.Settings.Queries.GetAllSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class SettingsController : ControllerBase
    {
        private readonly GetAllSettingsHandler _getAllHandler;
        private readonly UpdateSettingHandler _updateHandler;
        public SettingsController(
            GetAllSettingsHandler getAllHandler,
            UpdateSettingHandler updateHandler)
        {
            _getAllHandler = getAllHandler;
            _updateHandler = updateHandler;
        }

        [HttpGet]
        public async Task<ActionResult<List<GlobalSettingDto>>> GetAll(CancellationToken token)
        {
            var settings = await _getAllHandler.Handle(new GetAllSettingsQuery(), token).ConfigureAwait(false);
            return Ok(settings);
        }
        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateSettingRequest request, CancellationToken token)
        {
            await _updateHandler.Handle(new UpdateSettingCommand(key, request.Value), token).ConfigureAwait(false);
            return NoContent();
        }
    }
    public record UpdateSettingRequest(string Value);
}
