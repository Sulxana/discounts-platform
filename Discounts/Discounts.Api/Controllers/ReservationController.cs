using Discounts.Application.Common.Security;
using Discounts.Application.Reservations.Commands.CancelReservation;
using Discounts.Application.Reservations.Commands.CreateReservation;
using Discounts.Application.Reservations.Queries.GetUserReservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    [Authorize(Roles = Roles.Customer)]
    public class ReservationController : ControllerBase
    {
        private readonly CreateReservationHandler _createHandler;
        private readonly CancelReservationHandler _cancelHandler;
        private readonly GetUserReservationsHandler _getHandler;
        public ReservationController(CreateReservationHandler createHandler,CancelReservationHandler cancelHandler,GetUserReservationsHandler getHandler)
        {
            _createHandler = createHandler;
            _cancelHandler = cancelHandler;
            _getHandler = getHandler;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReservationDto>>> GetMyReservations(CancellationToken token)
        {
            var reservations = await _getHandler.GetUserReservations(token, new GetUserReservationsQuery());
            return Ok(reservations);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateReservationCommand command,
            CancellationToken token)
        {
            var reservationId = await _createHandler.CreateReservation(token, command);
            return Ok(reservationId);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken token)
        {
            await _cancelHandler.CancelReservationAsync(token, new CancelReservationCommand(id));
            return NoContent();
        }

    }
}
