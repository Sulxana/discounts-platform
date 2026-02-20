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

        /// <summary>
        /// Purchases an active reservation, converting it into coupons.
        /// </summary>
        /// <param name="reservationId">ID of the active reservation</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>List of created Coupon IDs</returns>
        [HttpPost("purchase/{reservationId}")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PurchaseReservation(Guid reservationId, CancellationToken token)
        {
            var command = new PurchaseReservationCommand { ReservationId = reservationId };
            var coupons = await _purchaseHandler.Handle(command, token);
            return Ok(coupons);
        }

        /// <summary>
        /// Purchases an offer directly without a prior reservation.
        /// </summary>
        /// <param name="offerId">ID of the offer</param>
        /// <param name="quantity">Number of coupons to purchase</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>List of created Coupon IDs</returns>
        [HttpPost("buy/{offerId}")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DirectPurchase(Guid offerId, [FromQuery] int quantity = 1, CancellationToken token = default)
        {
            var command = new DirectPurchaseCommand { OfferId = offerId, Quantity = quantity };
            var coupons = await _directPurchaseHandler.Handle(command, token);
            return Ok(coupons);
        }

        /// <summary>
        /// Retrieves the list of purchased coupons for the current user.
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>List of coupons</returns>
        [HttpGet("my")]
        [ProducesResponseType(typeof(List<Discounts.Domain.Coupons.Coupon>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCoupons(CancellationToken token)
        {
            var query = new GetMyCouponsQuery();
            var coupons = await _getCouponsHandler.Handle(query, token);
            return Ok(coupons);
        }
    }
}
