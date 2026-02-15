using Discounts.Domain.MerchantApplications;

namespace Discounts.Application.MerchantApplications.Interfaces
{
    public interface IMerchantApplicationRepository
    {
        Task<MerchantApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> HasPendingApplicationAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(MerchantApplication application, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
