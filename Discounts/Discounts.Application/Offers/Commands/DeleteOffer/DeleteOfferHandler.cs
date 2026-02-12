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
            var offer = await _repository.GetOfferByIdAsync(token, deleteOffer.Id);
            if (offer == null) throw new NotFoundException(nameof(Offer), deleteOffer.Id);

            //await _repository.DeleteOfferAsync(token, offer);

            offer.MarkAsDeleted();
            await _repository.SaveChangesAsync(token);
        }
    }
}
