using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;

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
            await _validator.ValidateAndThrowAsync(createOffer, token).ConfigureAwait(false);

            var userId = _currentUserService.UserId;
            if (userId == null) throw new UnauthorizedAccessException();

            var offer = new Offer(
                createOffer.Title,
                createOffer.Description,
                createOffer.CategoryId,
                createOffer.ImageUrl,
                createOffer.OriginalPrice,
                createOffer.DiscountedPrice,
                createOffer.TotalCoupons,
                createOffer.StartDate,
                createOffer.EndDate,
                userId.Value
            );

            await _repository.AddOfferAsync(token, offer).ConfigureAwait(false);
            await _repository.SaveChangesAsync(token).ConfigureAwait(false);

            return offer.Id;
        }
    }
}
