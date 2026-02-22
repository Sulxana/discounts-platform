using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;
        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        #region Methods
        public async Task<List<T>> GetAllAsync(CancellationToken token)
        {
            return await _dbSet.ToListAsync(cancellationToken: token).ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync(CancellationToken token, params object[] key)
        {
            return await _dbSet.FindAsync(key, token).ConfigureAwait(false);
        }
        public async Task Add(CancellationToken token, T entity)
        {
            await _dbSet.AddAsync(entity, token).ConfigureAwait(false);
            //await _context.SaveChangesAsync(token);
        }

        public async Task Update(CancellationToken token, T entity)
        {
            if (entity == null) return;

            _dbSet.Update(entity);
            //await _context.SaveChangesAsync(token);
        }

        public async Task Remove(CancellationToken token, T entity)
        {
            if (entity == null) return;

            _dbSet.Remove(entity);
            //await _context.SaveChangesAsync(token);
        }
        public async Task Remove(CancellationToken token, params object[] key)
        {
            var entity = await GetByIdAsync(token, key).ConfigureAwait(false);
            if (entity == null) return;

            _dbSet.Remove(entity);
            //await _context.SaveChangesAsync(token);
        }

        public async Task SaveChanges(CancellationToken token)
        {
            await _context.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public async Task<bool> AnyAsync(CancellationToken token, Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate, token).ConfigureAwait(false);
        }
        #endregion

    }
}
