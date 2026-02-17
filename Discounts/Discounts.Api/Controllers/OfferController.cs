using Asp.Versioning;
using Discounts.Application.Offers.Queries;
using Discounts.Application.Offers.Queries.GetActiveOffers;
using Discounts.Application.Offers.Queries.GetOfferById;
using Discounts.Domain.Offers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/offers")]
    [ApiController]
    [Authorize]
    public class OfferController : ControllerBase
    {
        private readonly GetOfferByIdHandler _getHandler;
        private readonly GetActiveOffersHandler _getActiveHandler;

        public OfferController(GetOfferByIdHandler getHandler, GetActiveOffersHandler getActiveHandler)
        {
            _getHandler = getHandler;
            _getActiveHandler = getActiveHandler;
        }

        [HttpGet]
        public async Task<ActionResult<List<OfferListItemDto>>> GetActiveOffers(CancellationToken token, [FromQuery] OfferCategory? category,
                                                                            [FromQuery] OfferStatus? status,
                                                                            [FromQuery] int page = 1,
                                                                            [FromQuery] int pageSize = 20)
        {
            var result = await _getActiveHandler.GetActiveOffers(token, new GetActiveOffersQuery(category, status, page, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OfferDetailsDto>> GetOfferById(CancellationToken token, Guid id)
        {
            var result = await _getHandler.GetOfferById(token, new GetOfferByIdQuery(id));
            if (result == null) return NotFound();

            return Ok(result);
        }

    }
}
