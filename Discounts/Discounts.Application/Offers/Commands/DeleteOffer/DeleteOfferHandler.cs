using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;

namespace Discounts.Application.Offers.Commands.DeleteOffer
{
    public class DeleteOfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<DeleteOfferCommand> _validator;
        private readonly ICouponRepository _couponRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteOfferHandler(IOfferRepository repository, IValidator<DeleteOfferCommand> validator, Discounts.Application.Coupons.Interfaces.ICouponRepository couponRepository, Discounts.Application.Common.Interfaces.ICurrentUserService currentUserService)
        {
            _repository = repository;
            _validator = validator;
            _couponRepository = couponRepository;
            _currentUserService = currentUserService;
        }

        public async Task DeleteOfferAsync(CancellationToken token, DeleteOfferCommand deleteOffer)
        {
            await _validator.ValidateAndThrowAsync(deleteOffer, token).ConfigureAwait(false);
            var offer = await _repository.GetOfferForUpdateByIdAsync(token, deleteOffer.Id).ConfigureAwait(false);
            if (offer == null) throw new NotFoundException(nameof(Offer), deleteOffer.Id);

            var userId = _currentUserService.UserId;

            if (userId == null || (userId != offer.MerchantId && !_currentUserService.IsInRole(Roles.Administrator)))
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this offer.");
            }

            var hasActiveReservations = offer.Reservations.Any(r => r.IsActive());
            if (hasActiveReservations)
                throw new InvalidOperationException("Cannot delete offer with active reservations. Please wait for them to expire or cancel them.");

            var hasSoldCoupons = await _couponRepository.HasCouponsForOfferAsync(offer.Id, token).ConfigureAwait(false);
            if (hasSoldCoupons)
                throw new InvalidOperationException("Cannot delete offer that has sold coupons.");

            //await _repository.DeleteOfferAsync(token, offer);

            offer.MarkAsDeleted(deleteOffer.Reason);
            await _repository.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}
