using Discounts.Application.Coupons.Commands.DirectPurchase;
using Discounts.Application.Coupons.Queries.GetMyCoupons;
using Discounts.Application.Reservations.Commands.PurchaseReservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Discounts.Application.Common.Security;

namespace Discounts.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(Roles = Roles.Customer)]
    public class CouponsController : ControllerBase
    {
        private readonly PurchaseReservationHandler _purchaseHandler;
        private readonly DirectPurchaseHandler _directPurchaseHandler;
        private readonly GetMyCouponsHandler _getCouponsHandler;

        public CouponsController(PurchaseReservationHandler purchaseHandler, DirectPurchaseHandler directPurchaseHandler, GetMyCouponsHandler getCouponsHandler)
        {
            _purchaseHandler = purchaseHandler;
            _directPurchaseHandler = directPurchaseHandler;
            _getCouponsHandler = getCouponsHandler;
        }

        [HttpPost("purchase/{reservationId}")]
        public async Task<IActionResult> PurchaseReservation(Guid reservationId, CancellationToken token)
        {
            var command = new PurchaseReservationCommand { ReservationId = reservationId };
            var coupons = await _purchaseHandler.Handle(command, token);
            return Ok(coupons);
        }

        [HttpPost("buy/{offerId}")]
        public async Task<IActionResult> DirectPurchase(Guid offerId, [FromQuery] int quantity = 1, CancellationToken token = default)
        {
            var command = new DirectPurchaseCommand { OfferId = offerId, Quantity = quantity };
            var coupons = await _directPurchaseHandler.Handle(command, token);
            return Ok(coupons);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyCoupons(CancellationToken token)
        {
            var query = new GetMyCouponsQuery();
            var coupons = await _getCouponsHandler.Handle(query, token);
            return Ok(coupons);
        }
    }
}
