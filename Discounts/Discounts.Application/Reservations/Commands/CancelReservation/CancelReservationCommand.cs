namespace Discounts.Application.Reservations.Commands.CancelReservation
{
    public class CancelReservationCommand
    {
        public Guid Id { get; set; }

        public CancelReservationCommand(Guid id)
        {
            Id = id;
        }
    }
}
