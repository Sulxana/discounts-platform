using Discounts.Application.Common.Interfaces;
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
        private readonly ICurrentUserService _currentUserService;

        public CreateOfferHandler(IOfferRepository repository, IValidator<CreateOfferCommand> validator, Discounts.Application.Common.Interfaces.ICurrentUserService currentUserService)
        {
            _repository = repository;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> CreateOffer(CancellationToken token, CreateOfferCommand createOffer)
        {
            await _validator.ValidateAndThrowAsync(createOffer, token);

            var offer = createOffer.Adapt<Offer>();
            var userId = _currentUserService.UserId;
            if (userId == null) throw new UnauthorizedAccessException();
            offer.SetMerchantId(userId.Value);

            await _repository.AddOfferAsync(token, offer);
            await _repository.SaveChangesAsync(token);

            return offer.Id;
        }
    }
}
