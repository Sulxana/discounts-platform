using Asp.Versioning;
using Discounts.Api.DTO;
using Discounts.Application.Common.Security;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.DeleteOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.Queries.GetMerchantDashboardStats;
using Discounts.Application.Offers.Queries.GetMerchantSalesHistory;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class MerchantController : ControllerBase
    {
        private readonly CreateOfferHandler _createHandler;
        private readonly UpdateOfferHandler _updateHandler;
        private readonly DeleteOfferHandler _deleteHandler;
        private readonly GetMerchantDashboardStatsHandler _getStatsHandler;
        private readonly GetMerchantSalesHistoryHandler _getSalesHistoryHandler;

        public MerchantController(CreateOfferHandler createHandler, UpdateOfferHandler updateHandler, DeleteOfferHandler deleteHandler, GetMerchantDashboardStatsHandler getStatsHandler, GetMerchantSalesHistoryHandler getSalesHistoryHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _getStatsHandler = getStatsHandler;
            _getSalesHistoryHandler = getSalesHistoryHandler;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Merchant)]
        public async Task<ActionResult<Guid>> Create(CancellationToken token, [FromBody] CreateOfferCommand command)
        {
            var result = await _createHandler.CreateOffer(token, command);
            return CreatedAtAction(nameof(OfferController.GetOfferById), "Offer", new { id = result }, result);
        }

        [HttpGet("dashboard-stats")]
        [Authorize(Roles = Roles.Merchant)]
        public async Task<ActionResult<MerchantDashboardStatsDto>> GetDashboardStats(CancellationToken token)
        {
            var result = await _getStatsHandler.Handle(new GetMerchantDashboardStatsQuery(), token);
            return Ok(result);
        }

        [HttpGet("sales-history")]
        [Authorize(Roles = Roles.Merchant)]
        public async Task<ActionResult<List<MerchantSalesHistoryDto>>> GetSalesHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken token = default)
        {
            var result = await _getSalesHistoryHandler.Handle(new GetMerchantSalesHistoryQuery(page, pageSize), token);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Administrator + "," + Roles.Merchant)]
        public async Task<IActionResult> UpdateOffer(CancellationToken token, Guid id, [FromBody] UpdateOfferRequestDto request)
        {
            var command = request.Adapt<UpdateOfferCommand>();
            command.Id = id;
            await _updateHandler.UpdateOfferAsync(token, command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Administrator + "," + Roles.Merchant)]
        public async Task<IActionResult> DeleteOffer(CancellationToken token, Guid id, [FromQuery] string? reason)
        {
            await _deleteHandler.DeleteOfferAsync(token, new DeleteOfferCommand(id, reason));
            return NoContent();
        }
    }
}
