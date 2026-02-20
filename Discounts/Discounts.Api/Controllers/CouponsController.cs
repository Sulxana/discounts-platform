using Discounts.Application.Coupons.Commands.DirectPurchase;
using Discounts.Application.Coupons.Queries.GetMyCoupons;
using Discounts.Application.Reservations.Commands.PurchaseReservation;
using Discounts.Application.Coupons.Commands.RedeemCoupon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Discounts.Application.Common.Security;

namespace Discounts.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CouponsController : ControllerBase
    {
        private readonly PurchaseReservationHandler _purchaseHandler;
        private readonly DirectPurchaseHandler _directPurchaseHandler;
        private readonly GetMyCouponsHandler _getCouponsHandler;
        private readonly RedeemCouponHandler _redeemCouponHandler;

        public CouponsController(PurchaseReservationHandler purchaseHandler, DirectPurchaseHandler directPurchaseHandler, GetMyCouponsHandler getCouponsHandler, RedeemCouponHandler redeemCouponHandler)
        {
            _purchaseHandler = purchaseHandler;
            _directPurchaseHandler = directPurchaseHandler;
            _getCouponsHandler = getCouponsHandler;
            _redeemCouponHandler = redeemCouponHandler;
        }

        /// <summary>
        /// Purchases an active reservation, converting it into coupons.
        /// </summary>
        /// <param name="reservationId">ID of the active reservation</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>List of created Coupon IDs</returns>
        [HttpPost("purchase/{reservationId}")]
        [Authorize(Roles = Roles.Customer)]
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
        [Authorize(Roles = Roles.Customer)]
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
        [Authorize(Roles = Roles.Customer)]
        [ProducesResponseType(typeof(List<Discounts.Domain.Coupons.Coupon>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCoupons(CancellationToken token)
        {
            var query = new GetMyCouponsQuery();
            var coupons = await _getCouponsHandler.Handle(query, token);
            return Ok(coupons);
        }

        /// <summary>
        /// Redeems a coupon by its unique code. Requires Merchant role.
        /// </summary>
        /// <param name="code">The 8-character coupon code</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Ok if successful</returns>
        [HttpPost("redeem/{code}")]
        [Authorize(Roles = Roles.Merchant)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RedeemCoupon(string code, CancellationToken token)
        {
            var command = new RedeemCouponCommand { Code = code };
            await _redeemCouponHandler.Handle(command, token);
            return Ok(new { Message = "Coupon successfully redeemed." });
        }
    }
}
