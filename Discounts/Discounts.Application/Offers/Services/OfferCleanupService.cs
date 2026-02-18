using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using Microsoft.Extensions.Logging;

namespace Discounts.Application.Offers.Services
{
    public class OfferCleanupService : IOfferCleanupService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OfferCleanupService> _logger;

        public OfferCleanupService(IOfferRepository offerRepository, IUnitOfWork unitOfWork, ILogger<OfferCleanupService> logger)
        {
            _offerRepository = offerRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> ProcessOffer(Guid offerId, CancellationToken token)
        {
            try
            {
                var offer = await _offerRepository.GetOfferForUpdateByIdAsync(token, offerId);
                if (offer == null)
                {
                    _logger.LogWarning($"Offer {offerId} not found during cleanup.");
                    return false;
                }

                if (offer.Status != OfferStatus.Approved && offer.Status != OfferStatus.Pending)
                {
                    _logger.LogInformation($"Offer {offerId} is not Approved or Pending. Status: {offer.Status}. Skipping.");
                    return true; 
                }

                if (offer.EndDate > DateTime.UtcNow)
                {
                    _logger.LogInformation($"Offer {offerId} is not expired yet. Skipping.");
                    return true;
                }

                offer.Expire();
                await _unitOfWork.SaveChangesAsync(token);
                
                _logger.LogInformation($"Successfully expired offer {offerId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to expire offer {offerId}.");
                throw;
            }
        }
    }
}
