using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Exceptions;
using Discounts.Application.MerchantApplications.Interfaces;
using Discounts.Domain.MerchantApplications;
using FluentValidation;

namespace Discounts.Application.MerchantApplications.Commands.ApplyMerchant
{
    public record ApplyMerchantCommand() ;

    public class ApplyMerchantHandler
    {
        private readonly IMerchantApplicationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public ApplyMerchantHandler(IMerchantApplicationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(ApplyMerchantCommand command, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User must be authenticated to apply.");
            }

            // Check for existing pending application
            var existingPending = await _repository.HasPendingApplicationAsync(userId.Value, token);

            if (existingPending)
            {
                throw new ValidationException("You already have a pending merchant application.");
            }

            var application = new MerchantApplication(userId.Value);

            await _repository.AddAsync(application, token);
            await _repository.SaveChangesAsync(token);

            return application.Id;
        }
    }
}
