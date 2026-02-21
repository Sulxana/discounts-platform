using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Discounts.Mvc.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveAsync(IFormFile file, CancellationToken token = default);
    }
}
