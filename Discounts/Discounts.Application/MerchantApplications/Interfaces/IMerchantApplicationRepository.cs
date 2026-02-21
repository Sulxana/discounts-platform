using Discounts.Domain.MerchantApplications;

namespace Discounts.Application.MerchantApplications.Interfaces
{
    public record MerchantApplicationWithUser(
        MerchantApplication Application,
        Guid UserId,
        string FirstName,
        string LastName,
        string Email
    );

    public interface IMerchantApplicationRepository
    {
        Task<MerchantApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<MerchantApplication?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> HasPendingApplicationAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(MerchantApplication application, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<List<MerchantApplicationWithUser>> GetAllWithUsersAsync(
            MerchantApplicationStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
