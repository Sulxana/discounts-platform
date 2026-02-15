using System.Security.Cryptography;
using System.Text;

namespace Discounts.Infrastracture.Auth
{
    public class TokenHasher
    {
        public static string Sha256Base64(string rawToken)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToBase64String(bytes);
        }
    }
}
