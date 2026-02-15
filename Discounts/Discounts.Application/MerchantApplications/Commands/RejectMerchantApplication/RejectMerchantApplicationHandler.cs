using Discounts.Application.Common.Exceptions;
using Discounts.Application.MerchantApplications.Interfaces;
using Discounts.Domain.MerchantApplications;

namespace Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication
{
    public record RejectMerchantApplicationCommand(Guid Id, string Reason);

    public class RejectMerchantApplicationHandler
    {
        private readonly IMerchantApplicationRepository _repository;

        public RejectMerchantApplicationHandler(IMerchantApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RejectMerchantApplicationCommand command, CancellationToken token)
        {
            var application = await _repository.GetByIdAsync(command.Id, token);

            if (application == null)
            {
                throw new NotFoundException(nameof(MerchantApplication), command.Id);
            }

            application.Reject(command.Reason);

            await _repository.SaveChangesAsync(token);
        }
    }
}
