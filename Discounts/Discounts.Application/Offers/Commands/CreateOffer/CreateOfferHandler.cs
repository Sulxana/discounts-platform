using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;
using Mapster;

namespace Discounts.Application.Offers.Commands.CreateOffer
{
    public class CreateOfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<CreateOfferCommand> _validator;

        public CreateOfferHandler(IOfferRepository repository, IValidator<CreateOfferCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<Guid> CreateOffer(CancellationToken token, CreateOfferCommand createOffer)
        {
            await _validator.ValidateAndThrowAsync(createOffer, token);

            var offer = createOffer.Adapt<Offer>();

            await _repository.AddAsync(token, offer);
            await _repository.SaveChangesAsync(token);

            return offer.Id;
        }
    }
}
