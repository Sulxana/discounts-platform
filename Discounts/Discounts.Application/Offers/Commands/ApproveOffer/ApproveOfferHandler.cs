using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;
using MediatR;

namespace Discounts.Application.Offers.Commands.ApproveOffer
{
    public class ApproveOfferHandler : IRequestHandler<ApproveOfferCommand>
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<ApproveOfferCommand> _validator;

        public ApproveOfferHandler(IOfferRepository repository, IValidator<ApproveOfferCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task Handle(ApproveOfferCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken).ConfigureAwait(false);

            var offer = await _repository.GetOfferForUpdateByIdAsync(cancellationToken, request.Id).ConfigureAwait(false);
            if (offer == null) throw new NotFoundException(nameof(Offer), request.Id);

            offer.Approve();
            await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

    }
}
