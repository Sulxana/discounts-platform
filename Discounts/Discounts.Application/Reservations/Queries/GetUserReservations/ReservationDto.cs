namespace Discounts.Application.Reservations.Queries.GetUserReservations
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public Guid OfferId { get; set; }
        public string OfferTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int MinutesRemaining { get; set; }
    }
}
