using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security; // For Roles
using Microsoft.AspNetCore.Identity;

namespace Discounts.Infrastracture.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool IsSuccess, string? Error, Guid UserId)> CreateUserAsync(string email, string password, string firstName, string lastName)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };

            var create = await _userManager.CreateAsync(user, password);
            if (!create.Succeeded)
            {
                return (false, string.Join(", ", create.Errors.Select(e => e.Description)), Guid.Empty);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded)
            {
                return (false, string.Join(", ", roleResult.Errors.Select(e => e.Description)), Guid.Empty);
            }

            return (true, null, user.Id);
        }

        public async Task<(bool IsSuccess, Guid UserId, string Email, IList<string> Roles)> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return (false, Guid.Empty, string.Empty, new List<string>());
            }

            var roles = await _userManager.GetRolesAsync(user);
            return (true, user.Id, user.Email!, roles);
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<(bool IsSuccess, Guid UserId, string Email, IList<string> Roles)> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, Guid.Empty, string.Empty, new List<string>());

            var check = await _userManager.CheckPasswordAsync(user, password);
            if (!check) return (false, Guid.Empty, string.Empty, new List<string>());

            var roles = await _userManager.GetRolesAsync(user);
            return (true, user.Id, user.Email!, roles);

        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
