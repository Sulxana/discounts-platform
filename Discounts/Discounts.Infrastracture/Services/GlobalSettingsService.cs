using Discounts.Application.Settings.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Infrastracture.Services
{
    public class GlobalSettingsService : IGlobalSettingsService
    {
        private readonly IGlobalSettingRepository _repository;
        private readonly IMemoryCache _cache;
        private const int CACHE_MINUTES = 5;

        public GlobalSettingsService(IGlobalSettingRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<string> GetStringAsync(string key, string defaultValue = "", CancellationToken token = default)
        {
            var value = await GetValueAsync(key, token);
            return value ?? defaultValue;
        }
        public async Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken cancellationToken = default)
        {
            var value = await GetValueAsync(key, cancellationToken);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }
        public async Task<decimal> GetDecimalAsync(string key, decimal defaultValue = 0m, CancellationToken cancellationToken = default)
        {
            var value = await GetValueAsync(key, cancellationToken);
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }
        public async Task<bool> GetBoolAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default)
        {
            var value = await GetValueAsync(key, cancellationToken);
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }
        private async Task<string?> GetValueAsync(string key, CancellationToken token)
        {
            
            if (_cache.TryGetValue(key, out string? cachedValue))
                return cachedValue;
            
            var setting = await _repository.GetByKeyAsync(token, key);

            if (setting == null)
                return null;
            
            _cache.Set(key, setting.Value, TimeSpan.FromMinutes(CACHE_MINUTES));

            return setting.Value;
        }

        public void RemoveFromCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
