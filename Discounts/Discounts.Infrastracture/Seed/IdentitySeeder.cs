using Discounts.Application.Common.Security;
using Discounts.Infrastracture.Identity;
using Microsoft.AspNetCore.Identity;

namespace Discounts.Infrastracture.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { Roles.Administrator, Roles.Merchant, Roles.Customer };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            var adminEmail = "super-admin@gmail.com";
            var merchantEmail = "merchant@example.com";
            var customerEmail = "customer@example.com";
            var defaultPassword = "Password123!";

            await CreateUserAsync(userManager, adminEmail, "Super", "Admin", Roles.Administrator, defaultPassword);
            await CreateUserAsync(userManager, merchantEmail, "Test", "Merchant", Roles.Merchant, defaultPassword);
            await CreateUserAsync(userManager, customerEmail, "Test", "Customer", Roles.Customer, defaultPassword);
        }

        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, string email, string firstName, string lastName, string role, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    UserName = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var create = await userManager.CreateAsync(user, password);
                if (!create.Succeeded)
                    throw new Exception(string.Join(", ", create.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
