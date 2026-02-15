using Discounts.Application.MerchantApplications.Interfaces;
using Discounts.Domain.MerchantApplications;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Persistence.Repositories
{
    public class MerchantApplicationRepository : IMerchantApplicationRepository
    {
        private readonly DiscountsDbContext _context;

        public MerchantApplicationRepository(DiscountsDbContext context)
        {
            _context = context;
        }

        public async Task<MerchantApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.MerchantApplications
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> HasPendingApplicationAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.MerchantApplications
                .AnyAsync(x => x.UserId == userId && x.Status == MerchantApplicationStatus.Pending, cancellationToken);
        }

        public async Task AddAsync(MerchantApplication application, CancellationToken cancellationToken)
        {
            await _context.MerchantApplications.AddAsync(application, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
