using Discounts.Application.Settings.Interfaces;
using Discounts.Domain.Settings;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Repositories
{
    public class GlobalSettingRepository : IGlobalSettingRepository
    {
        private readonly DiscountsDbContext _context;
        public GlobalSettingRepository(DiscountsDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalSetting?> GetByKeyAsync(CancellationToken cancellationToken, string key)
        {
            return await _context.GlobalSettings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<GlobalSetting>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.GlobalSettings
                .OrderBy(s => s.Key)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AddAsync(CancellationToken token, GlobalSetting setting)
        {
            await _context.GlobalSettings.AddAsync(setting, token).ConfigureAwait(false);
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
