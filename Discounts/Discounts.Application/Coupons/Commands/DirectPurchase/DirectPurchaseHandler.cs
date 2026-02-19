using Discounts.Application.Common.Interfaces;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Coupons;
using Discounts.Domain.Offers;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Discounts.Application.Coupons.Commands.DirectPurchase
{
    public class DirectPurchaseHandler
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DirectPurchaseHandler> _logger;
        private readonly IValidator<DirectPurchaseCommand> _validator;

        public DirectPurchaseHandler(IOfferRepository offerRepository, ICouponRepository couponRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<DirectPurchaseHandler> logger, FluentValidation.IValidator<DirectPurchaseCommand> validator)
        {
            _offerRepository = offerRepository;
            _couponRepository = couponRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _validator = validator;
        }

        public async Task<List<Guid>> Handle(DirectPurchaseCommand command, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(command, token);

            var userId = _currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User must be authenticated to purchase.");

            var offer = await _offerRepository.GetOfferForUpdateByIdAsync(token, command.OfferId);
            if (offer == null)
                throw new KeyNotFoundException($"Offer {command.OfferId} not found.");

            if (offer.Status != OfferStatus.Approved)
                throw new InvalidOperationException($"Offer {offer.Title} is not approved for purchase.");

            if (offer.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException($"Offer {offer.Title} has expired.");

            offer.DecreaseStock(command.Quantity);

            var coupons = new List<Coupon>();
            for (int i = 0; i < command.Quantity; i++)
            {
                var coupon = new Coupon(userId.Value, offer.Id, null);
                coupons.Add(coupon);
            }

            await _couponRepository.AddRangeAsync(token, coupons);

            await _unitOfWork.SaveChangesAsync(token);

            _logger.LogInformation($"User {userId} directly purchased {command.Quantity} coupons for offer {offer.Id}.");

            return coupons.Select(c => c.Id).ToList();
        }
    }
}
