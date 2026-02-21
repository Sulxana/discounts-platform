namespace Discounts.Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(Stream stream, string fileName, CancellationToken token);
    }
}
