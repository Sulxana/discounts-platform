namespace Discounts.Application.Reservations.Services
{
    public interface IReservationCleanupService
    {
        Task<bool> ProcessReservation(Guid reservationId, CancellationToken token);
    }
}
