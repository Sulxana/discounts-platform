namespace Discounts.Application.Settings.Interfaces
{
    public interface IGlobalSettingsService
    {
        Task<string> GetStringAsync(string key, string defaultValue = "", CancellationToken cancellationToken = default);
        Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken cancellationToken = default);
        Task<decimal> GetDecimalAsync(string key, decimal defaultValue = 0m, CancellationToken cancellationToken = default);
        Task<bool> GetBoolAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default);
        void RemoveFromCache(string key);
    }
}
