using Azure.Core;
using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Settings.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Domain.Settings;
using FluentValidation;
using Mapster;

namespace Discounts.Application.Offers.Commands.UpdateOffer
{
    public class UpdateOfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IValidator<UpdateOfferCommand> _validator;
        private readonly IGlobalSettingsService _settingsService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateOfferHandler(IOfferRepository repository, IValidator<UpdateOfferCommand> validator, IGlobalSettingsService settingsService, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _validator = validator;
            _settingsService = settingsService;
            _currentUserService = currentUserService;
        }

        public async Task UpdateOfferAsync(CancellationToken token, UpdateOfferCommand UpdateOffer)
        {
            await _validator.ValidateAndThrowAsync(UpdateOffer, token);

            var offer = await _repository.GetOfferByIdAsync(token, UpdateOffer.Id);
            if (offer == null) throw new NotFoundException(nameof(Offer), UpdateOffer.Id);

            var userId = _currentUserService.UserId;
            if (userId != offer.MerchantId && !_currentUserService.IsInRole(Roles.Administrator))
            {
                throw new UnauthorizedAccessException("You are not authorized to update this offer.");
            }

            var editWindowHours = await _settingsService.GetIntAsync(
                SettingKeys.MerchantEditWindowHours, defaultValue: 24, token);
            
            var cutoffTime = offer.CreatedAt.AddHours(editWindowHours);
            if (DateTime.UtcNow > cutoffTime)
                throw new InvalidOperationException($"Cannot edit offer after {editWindowHours} hours from creation");



            if (UpdateOffer.DiscountedPrice.HasValue && UpdateOffer.DiscountedPrice >= offer.OriginalPrice)
                throw new ValidationException("DiscountedPrice must be less than OriginalPrice.");

            if (UpdateOffer.EndDate.HasValue)
            {
                if (UpdateOffer.EndDate <= offer.StartDate)
                    throw new ValidationException("EndDate must be after StartDate.");

                if (UpdateOffer.EndDate <= DateTime.UtcNow)
                    throw new ValidationException("EndDate must be in the future.");
            }

            if (UpdateOffer.Title is null && UpdateOffer.Description is null && UpdateOffer.ImageUrl is null &&
                !UpdateOffer.DiscountedPrice.HasValue && !UpdateOffer.EndDate.HasValue)
            {
                throw new ValidationException("At least one field must be provided for update.");
            }

            offer.UpdateOfferFields(UpdateOffer.Title, UpdateOffer.Description, UpdateOffer.ImageUrl,
                                    UpdateOffer.DiscountedPrice, UpdateOffer.EndDate);

            await _repository.UpdateOfferAsync(token, offer);
            await _repository.SaveChangesAsync(token);
        }
    }
}
