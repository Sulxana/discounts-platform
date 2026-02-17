using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Interfaces;
using Discounts.Application.Settings.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Domain.Reservations;
using Discounts.Domain.Settings;
using FluentValidation;
using Mapster;

namespace Discounts.Application.Reservations.Commands.CreateReservation
{
    public class CreateReservationHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateReservationCommand> _validator;
        private readonly IGlobalSettingsService _settingsService;

        //private const int DEFAULT_RESERVATION_MINUTES = 30;

        public CreateReservationHandler(IReservationRepository reservationRepository, IOfferRepository offerRepository, ICurrentUserService currentUserService, IValidator<CreateReservationCommand> validator, IGlobalSettingsService settingsService)
        {
            _reservationRepository = reservationRepository;
            _offerRepository = offerRepository;
            _currentUserService = currentUserService;
            _validator = validator;
            _settingsService = settingsService;
        }



        public async Task<Guid> CreateReservation(CancellationToken token, CreateReservationCommand command)
        {
            await _validator.ValidateAndThrowAsync(command, token);

            var userId = _currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User must be authenticated");

            var offer = await _offerRepository.GetOfferByIdAsync(token, command.OfferId);
            if (offer == null)
                throw new InvalidOperationException("Offer not found");

            if (offer.Status != OfferStatus.Approved)
                throw new InvalidOperationException("Only approved offers can be reserved");

            if (offer.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException("This offer has expired");

            var hasActiveReservation = await _reservationRepository.HasActiveReservationForOfferAsync(userId.Value, command.OfferId, token);

            if (hasActiveReservation)
                throw new InvalidOperationException("You already have an active reservation for this offer");

            if (offer.RemainingCoupons < command.Quantity)
                throw new InvalidOperationException($"only {offer.RemainingCoupons} coupons are available");

            offer.DecreaseStock(command.Quantity);

            var expirationMinutes = await _settingsService.GetIntAsync(
               SettingKeys.ReservationExpirationMinutes, defaultValue: 30, token);


            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var reservation = new Reservation(userId.Value, command.OfferId, command.Quantity, expiresAt);

            await _reservationRepository.AddAsync(token, reservation);
            await _reservationRepository.SaveChangesAsync(token);
            await _offerRepository.SaveChangesAsync(token);

            return reservation.Id;
        }
    }
}
