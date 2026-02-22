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
        private readonly IUnitOfWork _unitOfWork;

        public CancelReservationHandler(IReservationRepository reservationRepository, IOfferRepository offerRepository, ICurrentUserService currentUserService, IValidator<CancelReservationCommand> validator, IUnitOfWork unitOfWork)
        {
            _reservationRepository = reservationRepository;
            _offerRepository = offerRepository;
            _currentUserService = currentUserService;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task CancelReservationAsync(CancellationToken token, CancelReservationCommand command)
        {
            await _validator.ValidateAndThrowAsync(command, token).ConfigureAwait(false);

            var reservation = await _reservationRepository.GetByIdAsync(token, command.Id).ConfigureAwait(false);
            if (reservation == null)
                throw new NotFoundException("Reservation not found");

            var userId = _currentUserService.UserId;
            // Fix: Ensure userId is not null and matches
            if (userId == null || reservation.UserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own reservations.");

            reservation.Cancel();

            var offer = await _offerRepository.GetOfferForUpdateByIdAsync(token, reservation.OfferId).ConfigureAwait(false);
            offer?.IncreaseStock(reservation.Quantity);

            // Old Logic:
            // await _offerRepository.SaveChangesAsync(token);
            // await _reservationRepository.SaveChangesAsync(token);

            // New Logic:
            await _unitOfWork.SaveChangesAsync(token).ConfigureAwait(false);
        }

    }
}
