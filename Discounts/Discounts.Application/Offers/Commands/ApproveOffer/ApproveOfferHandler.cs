using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;

namespace Discounts.Application.Offers.Commands.ApproveOffer
{
    public class ApproveOfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<ApproveOfferCommand> _validator;

        public ApproveOfferHandler(IOfferRepository repository, IValidator<ApproveOfferCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task ApproveOfferAsync(CancellationToken token, ApproveOfferCommand approvedOffer)
        {
            await _validator.ValidateAndThrowAsync(approvedOffer, token);

            var offer = await _repository.GetOfferForUpdateByIdAsync(token, approvedOffer.Id);
            if (offer == null) throw new NotFoundException(nameof(Offer), approvedOffer.Id);

            offer.Approve();
            await _repository.SaveChangesAsync(token);
        }

    }
}
