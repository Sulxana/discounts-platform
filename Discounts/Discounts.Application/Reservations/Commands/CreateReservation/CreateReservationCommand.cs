namespace Discounts.Application.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommand
    {
        public Guid OfferId { get; set; }
        public int Quantity { get; set; }
    }
}
