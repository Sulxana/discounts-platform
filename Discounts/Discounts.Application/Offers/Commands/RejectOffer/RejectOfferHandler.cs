using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;

namespace Discounts.Application.Offers.Commands.RejectOffer
{
    public class RejectOfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<RejectOfferCommand> _validator;

        public RejectOfferHandler(IOfferRepository repository, IValidator<RejectOfferCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task RejectOfferAsync(CancellationToken token, RejectOfferCommand rejectOffer)
        {
            await _validator.ValidateAndThrowAsync(rejectOffer, token).ConfigureAwait(false);

            var offer = await _repository.GetOfferForUpdateByIdAsync(token, rejectOffer.Id).ConfigureAwait(false);
            if (offer == null) throw new NotFoundException(nameof(Offer), rejectOffer.Id);

            offer.Reject(rejectOffer.Reason);
            await _repository.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}
