using Discounts.Domain.Categories;

namespace Discounts.Application.Categories.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddCategoryAsync(CancellationToken token, Category category);
        Task<Category?> GetCategoryByIdAsync(CancellationToken token, Guid id);
        Task<List<Category>> GetAllCategoriesAsync(CancellationToken token);
        Task UpdateCategoryAsync(CancellationToken token, Category category);
        Task DeleteCategoryAsync(CancellationToken token, Category category);
        Task SaveChangesAsync(CancellationToken token);
    }
}
