using FluentValidation;

namespace Discounts.Application.Reservations.Commands.CancelReservation
{
    public class CancelReservationValidator:AbstractValidator<CancelReservationCommand>
    {
        public CancelReservationValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
