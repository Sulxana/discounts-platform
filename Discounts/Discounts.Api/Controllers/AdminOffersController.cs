using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Queries;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Application.Offers.Queries.GetDeletedOffers;
using Discounts.Application.Offers.Queries.GetOfferById;
using Discounts.Domain.Offers;
using Microsoft.AspNetCore.Authorization;
using Discounts.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Discounts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class AdminOffersController : ControllerBase
    {
        private readonly GetAllOffersHandler _getAllHandler;
        private readonly GetDeletedOffersHandler _getDeletedHandler;
        private readonly GetOfferByIdHandler _getHandler;
        private readonly ApproveOfferHandler _approveHandler;
        private readonly RejectOfferHandler _rejectHandler;

        public AdminOffersController(GetAllOffersHandler getAllHandler, GetDeletedOffersHandler getDeletedHandler, GetOfferByIdHandler getHandler, ApproveOfferHandler approveHandler, RejectOfferHandler rejectHandler)
        {
            _getAllHandler = getAllHandler;
            _getDeletedHandler = getDeletedHandler;
            _getHandler = getHandler;
            _approveHandler = approveHandler;
            _rejectHandler = rejectHandler;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OfferDetailsDto>> GetOfferIncludingDeletedAsync(CancellationToken token, Guid id)
        {
            var result = await _getHandler.GetOfferIncludingDeletedAsync(token, new GetOfferByIdQuery(id));
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("AllOffers")]
        public async Task<ActionResult<List<OfferListItemDto>>> GetAllOffers(CancellationToken token, [FromQuery] OfferCategory? category,
                                                                            [FromQuery] OfferStatus? status, [FromQuery] bool deleted,
                                                                            [FromQuery] int page = 1,
                                                                            [FromQuery] int pageSize = 20)
        {
            var result = await _getAllHandler.GetAllOffers(token, new GetAllOffersQuery(category, status, deleted, page, pageSize));
            return Ok(result);
        }

        [HttpGet("DeletedOffers")]
        public async Task<ActionResult<List<OfferListItemDto>>> GetDeletedOffers(CancellationToken token, [FromQuery] OfferCategory? category,
                                                                            [FromQuery] OfferStatus? status,
                                                                            [FromQuery] int page = 1,
                                                                            [FromQuery] int pageSize = 20)
        {
            var result = await _getDeletedHandler.GetDeletedOffers(token, new GetDeletedOffersQuery(category, status, page, pageSize));
            return Ok(result);
        }

        [HttpPut("ApproveOffer/{id:guid}")]
        public async Task<IActionResult> ApproveOffer(CancellationToken token, Guid id)
        {
            await _approveHandler.ApproveOfferAsync(token, new ApproveOfferCommand(id));
            return NoContent();
        }

        [HttpPut("RejectOffer/{id:guid}")]
        public async Task<IActionResult> RejectOffer(CancellationToken token, Guid id, [FromQuery][Required] string reason)
        {
            await _rejectHandler.RejectOfferAsync(token, new RejectOfferCommand(id, reason));
            return NoContent();
        }
    }
}
