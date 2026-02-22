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
            var value = await GetValueAsync(key, token).ConfigureAwait(false);
            return value ?? defaultValue;
        }
        public async Task<int> GetIntAsync(string key, int defaultValue = 0, CancellationToken cancellationToken = default)
        {
            var value = await GetValueAsync(key, cancellationToken).ConfigureAwait(false);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }
        public async Task<decimal> GetDecimalAsync(string key, decimal defaultValue = 0m, CancellationToken cancellationToken = default)
        {
            var value = await GetValueAsync(key, cancellationToken).ConfigureAwait(false);
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }
        public async Task<bool> GetBoolAsync(string key, bool defaultValue = false, CancellationToken cancellationToken = default)
        {
            var value = await GetValueAsync(key, cancellationToken).ConfigureAwait(false);
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }
        private static string NormalizeKey(string key) => key?.Trim().ToLowerInvariant() ?? string.Empty;

        private async Task<string?> GetValueAsync(string key, CancellationToken token)
        {
            var normalizedKey = NormalizeKey(key);

            if (_cache.TryGetValue(normalizedKey, out string? cachedValue))
                return cachedValue;

            var setting = await _repository.GetByKeyAsync(token, normalizedKey).ConfigureAwait(false);

            if (setting == null)
                return null;

            _cache.Set(normalizedKey, setting.Value, TimeSpan.FromMinutes(CACHE_MINUTES));

            return setting.Value;
        }

        public void RemoveFromCache(string key)
        {
            var normalizedKey = NormalizeKey(key);
            _cache.Remove(normalizedKey);
        }
    }
}
