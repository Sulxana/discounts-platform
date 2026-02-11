using Discounts.Api.DTO.Offers;
using Discounts.Application.Offers.Commands.CreateOffer;
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

        public OfferController(CreateOfferHandler handler, GetOfferByIdHandler getHandler)
        {
            _createHandler = handler;
            _getHandler = getHandler;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetOfferDetailsResponse>> GetOfferById(CancellationToken token, Guid id)
        {
            var dto = await _getHandler.GetOfferById(token, new GetOfferByIdQuery(id));
            var response = dto.Adapt<GetOfferDetailsResponse>();

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CancellationToken cancellationToken, [FromBody] CreateOfferRequestDto request)
        {
            var command = request.Adapt<CreateOfferCommand>();

            var result = await _createHandler.CreateOffer(cancellationToken, command);

            return Created($"{result}", new { result });
        }
    }
}
