using Discounts.Domain.Auth;
using Microsoft.AspNetCore.Identity;

namespace Discounts.Infrastracture.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }

}
