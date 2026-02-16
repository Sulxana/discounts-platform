using Discounts.Api.DTO;
using Discounts.Application.Common.Security;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.DeleteOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =Roles.Merchant)]
    public class MerchantController : ControllerBase
    {
        private readonly CreateOfferHandler _createHandler;
        private readonly UpdateOfferHandler _updateHandler;
        private readonly DeleteOfferHandler _deleteHandler;

        public MerchantController(CreateOfferHandler createHandler, UpdateOfferHandler updateHandler, DeleteOfferHandler deleteHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CancellationToken token, [FromBody] CreateOfferCommand command)
        {
            var result = await _createHandler.CreateOffer(token, command);

            return CreatedAtAction("GetOfferById", new { id = result }, result);
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
