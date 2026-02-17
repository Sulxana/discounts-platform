using Discounts.Domain.Settings;

namespace Discounts.Application.Settings.Interfaces
{
    public interface IGlobalSettingRepository
    {
        Task<GlobalSetting?> GetByKeyAsync(CancellationToken token, string key);
        Task<List<GlobalSetting>> GetAllAsync(CancellationToken token);
        Task AddAsync(CancellationToken token, GlobalSetting setting);
        Task SaveChangesAsync(CancellationToken token);
    }
}
