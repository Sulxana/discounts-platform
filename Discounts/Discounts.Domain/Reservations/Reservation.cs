namespace Discounts.Domain.Reservations
{
    public class Reservation
    {
        private Reservation()
        {
        }

        public Reservation(Guid userId, Guid offerId, int quantity, DateTime expiresAt)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive number");
            if (expiresAt <= CreatedAt)
                throw new ArgumentOutOfRangeException("CreatedAt must be less than expiredAt");
            if (userId == Guid.Empty)
                throw new ArgumentException("user id must not be empty");
            if (offerId == Guid.Empty)
                throw new ArgumentException("offer id must not be empty");

            Id = Guid.NewGuid();
            UserId = userId;
            OfferId = offerId;
            Quantity = quantity;
            ExpiresAt = expiresAt;
            Status = ReservationStatus.Active;
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid OfferId { get; private set; }
        public int Quantity { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; private set; }
        public ReservationStatus Status { get; private set; }

        public bool IsActive()
        {
            return Status == ReservationStatus.Active;
        }
        public bool IsExpired()
        {
            return Status == ReservationStatus.Expired;
        }
        public bool IsCompleted()
        {
            return Status == ReservationStatus.Completed;
        }
        public void Cancel()
        {
            if (!IsActive())
                throw new InvalidOperationException("Reservation is not active");

            Status = ReservationStatus.Cancelled;
        }

        public void MarkAsExpired(DateTime utcNow)
        {
            if (!IsActive())
                throw new InvalidOperationException("Only active reservations can expire");

            if (IsActive() && utcNow >= ExpiresAt)
                Status = ReservationStatus.Expired;

        }

        public void MarkAsCompleted()
        {
            if (!IsActive())
                throw new InvalidOperationException("Reservation is not active");
            Status = ReservationStatus.Completed;
        }
    }
}
