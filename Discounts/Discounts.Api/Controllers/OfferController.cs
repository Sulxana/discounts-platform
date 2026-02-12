using Discounts.Api.DTO;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.DeleteOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.Queries;
using Discounts.Application.Offers.Queries.GetActiveOffers;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Application.Offers.Queries.GetDeletedOffers;
using Discounts.Application.Offers.Queries.GetOfferById;
using Discounts.Domain.Offers;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly CreateOfferHandler _createHandler;
        private readonly GetOfferByIdHandler _getHandler;
        private readonly UpdateOfferHandler _updateHandler;
        private readonly GetActiveOffersHandler _getActiveHandler;
        
        private readonly DeleteOfferHandler _deleteHandler;

        public OfferController(CreateOfferHandler createHandler, GetOfferByIdHandler getHandler, UpdateOfferHandler updateHandler, GetActiveOffersHandler getActiveHandler, GetAllOffersHandler getAllHandler, GetDeletedOffersHandler getDeletedHandler, DeleteOfferHandler deleteHandler)
        {
            _createHandler = createHandler;
            _getHandler = getHandler;
            _updateHandler = updateHandler;
            _getActiveHandler = getActiveHandler;
            _deleteHandler = deleteHandler;
        }

        [HttpGet("ActiveOffers")]
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

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CancellationToken token, [FromBody] CreateOfferCommand command)
        {
            var result = await _createHandler.CreateOffer(token, command);

            return CreatedAtAction(nameof(GetOfferById), new { id = result }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateOffer(CancellationToken token, Guid id, [FromBody] UpdateOfferRequestDto request)
        {
            var command = request.Adapt<UpdateOfferCommand>();
            command.Id = id;
            await _updateHandler.UpdateOfferAsync(token, command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOffer(CancellationToken token, Guid id)
        {
            await _deleteHandler.DeleteOfferAsync(token, new DeleteOfferCommand(id));
            return NoContent();
        }
    }
}
