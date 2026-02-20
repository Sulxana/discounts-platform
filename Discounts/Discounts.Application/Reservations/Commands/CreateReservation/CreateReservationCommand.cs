namespace Discounts.Application.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommand
    {
        /// <summary>
        /// The ID of the offer to reserve
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid OfferId { get; set; }

        /// <summary>
        /// The number of coupons to reserve
        /// </summary>
        /// <example>2</example>
        public int Quantity { get; set; }
    }
}
