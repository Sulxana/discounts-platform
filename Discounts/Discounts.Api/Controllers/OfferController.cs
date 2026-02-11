using Discounts.Api.DTO.Offers;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.DTO.Offer;
using Discounts.Application.Offers.Queries.GetOfferById;
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

        public OfferController(CreateOfferHandler createHandler, GetOfferByIdHandler getHandler, UpdateOfferHandler updateHandler)
        {
            _createHandler = createHandler;
            _getHandler = getHandler;
            _updateHandler = updateHandler;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetOfferDetailsResponse>> GetOfferById(CancellationToken token, Guid id)
        {
            var dto = await _getHandler.GetOfferById(token, new GetOfferByIdQuery(id));
            var response = dto.Adapt<GetOfferDetailsResponse>();

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CancellationToken token, [FromBody] CreateOfferRequestDto request)
        {
            var command = request.Adapt<CreateOfferCommand>();

            var result = await _createHandler.CreateOffer(token, command);

            return Created($"{result}", new { result });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateOffer(CancellationToken token, Guid id, [FromBody] UpdateOfferRequestDto request)
        {
            var command = request.Adapt<UpdateOfferCommand>();
            command.Id = id;
            await _updateHandler.UpdateOfferAsync(token, command);
            return NoContent();
        }
    }
}
