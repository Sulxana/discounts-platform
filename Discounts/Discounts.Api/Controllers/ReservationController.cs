using Asp.Versioning;
using Discounts.Application.Common.Security;
using Discounts.Application.Reservations.Commands.CancelReservation;
using Discounts.Application.Reservations.Commands.CreateReservation;
using Discounts.Application.Reservations.Queries.GetUserReservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reservations")]
    [ApiController]
    [Authorize(Roles = Roles.Customer)]
    public class ReservationController : ControllerBase
    {
        private readonly CreateReservationHandler _createHandler;
        private readonly CancelReservationHandler _cancelHandler;
        private readonly GetUserReservationsHandler _getHandler;
        public ReservationController(CreateReservationHandler createHandler, CancelReservationHandler cancelHandler, GetUserReservationsHandler getHandler)
        {
            _createHandler = createHandler;
            _cancelHandler = cancelHandler;
            _getHandler = getHandler;
        }

        /// <summary>
        /// Retrieves the list of active/completed reservations for the current customer.
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>List of reservations</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReservationDto>>> GetMyReservations(CancellationToken token)
        {
            var reservations = await _getHandler.GetUserReservations(token, new GetUserReservationsQuery()).ConfigureAwait(false);
            return Ok(reservations);
        }

        /// <summary>
        /// Creates a new reservation for an offer.
        /// </summary>
        /// <param name="command">Reservation details (e.g. OfferId, Quantity)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The ID of the newly created reservation</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateReservationCommand command,
            CancellationToken token)
        {
            var reservationId = await _createHandler.CreateReservation(token, command).ConfigureAwait(false);
            return Ok(reservationId);
        }

        /// <summary>
        /// Cancels an existing, active reservation.
        /// </summary>
        /// <param name="id">The ID of the reservation to cancel</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken token)
        {
            await _cancelHandler.CancelReservationAsync(token, new CancelReservationCommand(id)).ConfigureAwait(false);
            return NoContent();
        }

    }
}
