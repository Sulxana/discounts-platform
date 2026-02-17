using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;

namespace Discounts.Application.Offers.Commands.DeleteOffer
{
    public class DeleteOfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<DeleteOfferCommand> _validator;

        public DeleteOfferHandler(IOfferRepository repository, IValidator<DeleteOfferCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task DeleteOfferAsync(CancellationToken token, DeleteOfferCommand deleteOffer)
        {
            var offer = await _repository.GetOfferForUpdateByIdAsync(token, deleteOffer.Id);
            if (offer == null) throw new NotFoundException(nameof(Offer), deleteOffer.Id);

            var hasActiveReservations = offer.Reservations.Any(r => r.IsActive());
            if (hasActiveReservations)
                throw new InvalidOperationException("Cannot delete offer with active reservations. Please wait for them to expire or cancel them.");

            //await _repository.DeleteOfferAsync(token, offer);

            offer.MarkAsDeleted(deleteOffer.Reason);
            await _repository.SaveChangesAsync(token);
        }
    }
}
