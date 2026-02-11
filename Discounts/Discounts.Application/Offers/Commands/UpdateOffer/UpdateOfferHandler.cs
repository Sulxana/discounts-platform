using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentValidation;
using Mapster;

namespace Discounts.Application.Offers.Commands.UpdateOffer
{
    public class UpdateOfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<UpdateOfferCommand> _validator;

        public UpdateOfferHandler(IOfferRepository repository, IValidator<UpdateOfferCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task UpdateOfferAsync(CancellationToken token, UpdateOfferCommand UpdateOffer)
        {
            await _validator.ValidateAndThrowAsync(UpdateOffer, token);

            var offer = await _repository.GetOfferAsync(token, UpdateOffer.Id);
            if (offer == null) throw new NotFoundException(nameof(Offer), UpdateOffer.Id);

            if (UpdateOffer.EndDate <= offer.StartDate)
                throw new ValidationException("EndDate must be after StartDate.");

            if (UpdateOffer.EndDate <= DateTime.UtcNow)
                throw new ValidationException("EndDate must be in the future.");

            if (UpdateOffer.DiscountedPrice >= offer.OriginalPrice)
                throw new ValidationException("DiscountedPrice must be less than OriginalPrice.");


            offer.UpdateOfferFields(UpdateOffer.Title, UpdateOffer.Description, UpdateOffer.ImageUrl,
                                    UpdateOffer.DiscountedPrice, UpdateOffer.EndDate);


            //await _repository.UpdateAsync(token, offer);
            await _repository.SaveChangesAsync(token);
        }
    }
}
