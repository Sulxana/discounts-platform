namespace Discounts.Mvc.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveAsync(IFormFile file, CancellationToken token = default);
    }
}
