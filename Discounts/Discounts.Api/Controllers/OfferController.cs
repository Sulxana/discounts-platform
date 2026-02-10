using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.DTO.Offer;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly CreateOfferHandler _handler;

        public OfferController(CreateOfferHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CancellationToken cancellationToken, [FromBody] CreateOfferRequestDto request)
        {
            var command = request.Adapt<CreateOfferCommand>();

            var result = await _handler.CreateOffer(cancellationToken, command);

            return Created($"{result}", new { result});
        }
    }
}
