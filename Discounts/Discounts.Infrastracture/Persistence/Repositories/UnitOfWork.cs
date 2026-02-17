using Discounts.Application.Common.Interfaces;
using Discounts.Infrastracture.Persistence.Context;

namespace Discounts.Infrastracture.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DiscountsDbContext _context;

        public UnitOfWork(DiscountsDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token);
        }
    }
}
