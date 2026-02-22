using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Application.MerchantApplications.Interfaces;
using Discounts.Domain.MerchantApplications;

namespace Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication
{
    public record ApproveMerchantApplicationCommand(Guid Id);

    public class ApproveMerchantApplicationHandler
    {
        private readonly IMerchantApplicationRepository _repository;
        private readonly IIdentityService _identityService;

        public ApproveMerchantApplicationHandler(IMerchantApplicationRepository repository, IIdentityService identityService)
        {
            _repository = repository;
            _identityService = identityService;
        }

        public async Task Handle(ApproveMerchantApplicationCommand command, CancellationToken token)
        {
            var application = await _repository.GetByIdAsync(command.Id, token).ConfigureAwait(false);

            if (application == null)
            {
                throw new NotFoundException(nameof(MerchantApplication), command.Id);
            }

            application.Approve();

            // Assign Merchant Role
            await _identityService.AddRoleAsync(application.UserId, Roles.Merchant).ConfigureAwait(false);

            // Remove Customer Role (per user preference)
            await _identityService.RemoveRoleAsync(application.UserId, Roles.Customer).ConfigureAwait(false);

            await _repository.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}
