using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Discounts.Application.Interfaces;

namespace Discounts.Infrastracture.Images
{
    public class CloudinaryImageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadAsync(Stream stream, string fileName, CancellationToken token)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = "discounts-offers",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }
    }
}
