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
        private readonly MediatR.ISender _mediator;

        public OfferController(GetOfferByIdHandler getHandler, MediatR.ISender mediator)
        {
            _getHandler = getHandler;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves paginated list of active (Approved and Not Expired) offers.
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="category">Filter by category name</param>
        /// <param name="status">Filter by status (ignored for public active offers)</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <returns>List of active offers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OfferListItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OfferListItemDto>>> GetActiveOffers(CancellationToken token, [FromQuery] string? category,
                                                                            [FromQuery] OfferStatus? status,
                                                                            [FromQuery] int page = 1,
                                                                            [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetActiveOffersQuery(category, null, null, null, status, page, pageSize), token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves details of a specific offer by ID.
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="id">Offer ID</param>
        /// <returns>Offer details or NotFound</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OfferDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OfferDetailsDto>> GetOfferById(CancellationToken token, Guid id)
        {
            var result = await _getHandler.GetOfferById(token, new GetOfferByIdQuery(id)).ConfigureAwait(false);
            if (result == null) return NotFound();

            return Ok(result);
        }

    }
}
