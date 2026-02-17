using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Interfaces;
using FluentValidation;

namespace Discounts.Application.Reservations.Commands.CancelReservation
{
    public class CancelReservationHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CancelReservationCommand> _validator;

        public CancelReservationHandler(IReservationRepository reservationRepository, IOfferRepository offerRepository, ICurrentUserService currentUserService, IValidator<CancelReservationCommand> validator)
        {
            _reservationRepository = reservationRepository;
            _offerRepository = offerRepository;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task CancelReservationAsync(CancellationToken token, CancelReservationCommand command)
        {
            await _validator.ValidateAndThrowAsync(command, token);

            var reservation = await _reservationRepository.GetByIdAsync(token, command.Id);
            if (reservation == null)
                throw new NotFoundException("Reservation not found");

            var userId = _currentUserService.UserId;
            if (reservation.UserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own reservations.");

            reservation.Cancel();

            var offer = await _offerRepository.GetOfferForUpdateByIdAsync(token, reservation.OfferId);
            if (offer != null)
            {
                offer.IncreaseStock(reservation.Quantity);
            }

            await _offerRepository.SaveChangesAsync(token);
            await _reservationRepository.SaveChangesAsync(token);
        }

    }
}
