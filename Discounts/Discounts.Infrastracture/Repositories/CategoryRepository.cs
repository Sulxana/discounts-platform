using Discounts.Application.Categories.Interfaces;
using Discounts.Domain.Categories;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DiscountsDbContext _context;

        public CategoryRepository(DiscountsDbContext context)
        {
            _context = context;
        }

        public async Task AddCategoryAsync(CancellationToken token, Category category)
        {
            await _context.Categories.AddAsync(category, token);
        }

        public async Task<Category?> GetCategoryByIdAsync(CancellationToken token, Guid id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken token)
        {
            return await _context.Categories.ToListAsync(token);
        }

        public Task UpdateCategoryAsync(CancellationToken token, Category category)
        {
            _context.Categories.Update(category);
            return Task.CompletedTask;
        }

        public Task DeleteCategoryAsync(CancellationToken token, Category category)
        {
            _context.Categories.Remove(category);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await _context.SaveChangesAsync(token);
        }
    }
}
