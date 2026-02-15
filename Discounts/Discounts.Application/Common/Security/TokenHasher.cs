using System.Security.Cryptography;
using System.Text;

namespace Discounts.Application.Common.Security
{
    public static class TokenHasher
    {
        public static string Sha256Base64(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
