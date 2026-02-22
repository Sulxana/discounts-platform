using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool IsSuccess, string? Error, Guid UserId)> CreateUserAsync(string email, string password, string firstName, string lastName, string role)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return (false, result.Errors.First().Description, Guid.Empty);
            }

            await _userManager.AddToRoleAsync(user, role).ConfigureAwait(false);

            return (true, null, user.Id);
        }

        public async Task<(bool IsSuccess, Guid UserId, string Email, IList<string> Roles)> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

            if (user == null)
            {
                return (false, Guid.Empty, string.Empty, new List<string>());
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return (false, Guid.Empty, string.Empty, new List<string>());
            }

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            return (true, user.Id, user.Email!, roles);
        }

        public async Task<(bool IsSuccess, Guid UserId, string Email, IList<string> Roles)> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null) return (false, Guid.Empty, string.Empty, new List<string>());

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            return (true, user.Id, user.Email!, roles);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email).ConfigureAwait(false) != null;
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        }

        public async Task AddRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            }
        }

        public async Task RemoveRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
            }
        }

        public async Task<bool> BlockUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null) throw new NotFoundException(nameof(ApplicationUser), userId);

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100)).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<bool> UnblockUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null) throw new NotFoundException(nameof(ApplicationUser), userId);

            var result = await _userManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(Guid userId, string? email, string? firstName, string? lastName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null) throw new NotFoundException(nameof(ApplicationUser), userId);

            if (!string.IsNullOrWhiteSpace(email) && email != user.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, email).ConfigureAwait(false);
                if (!emailResult.Succeeded) return false;
                user.UserName = email;
            }

            if (!string.IsNullOrWhiteSpace(firstName)) user.FirstName = firstName;
            if (!string.IsNullOrWhiteSpace(lastName)) user.LastName = lastName;

            var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<List<(Guid Id, string Email, string FirstName, string LastName, bool IsBlocked, IList<string> Roles)>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
            var userList = new List<(Guid Id, string Email, string FirstName, string LastName, bool IsBlocked, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var isBlocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;
                userList.Add((user.Id, user.Email!, user.FirstName, user.LastName, isBlocked, roles));
            }

            return userList;
        }
    }
}
