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
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<MerchantApplication?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.MerchantApplications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> HasPendingApplicationAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.MerchantApplications
                .AnyAsync(x => x.UserId == userId && x.Status == MerchantApplicationStatus.Pending, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddAsync(MerchantApplication application, CancellationToken cancellationToken)
        {
            await _context.MerchantApplications.AddAsync(application, cancellationToken).ConfigureAwait(false);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<MerchantApplicationWithUser>> GetAllWithUsersAsync(
            MerchantApplicationStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.MerchantApplications
                .Join(_context.Users,
                    application => application.UserId,
                    user => user.Id,
                    (application, user) => new { Application = application, User = user });

            // filter by status
            if (status.HasValue)
            {
                query = query.Where(x => x.Application.Status == status.Value);
            }

            var results = await query
                .OrderByDescending(x => x.Application.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return results.Select(x => new MerchantApplicationWithUser(
                x.Application,
                x.User.Id,
                x.User.FirstName,
                x.User.LastName,
                x.User.Email ?? string.Empty
            )).ToList();
        }
    }
}
