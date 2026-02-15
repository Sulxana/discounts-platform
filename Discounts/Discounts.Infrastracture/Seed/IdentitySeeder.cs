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
            var adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = adminEmail,
                    UserName = adminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var create = await userManager.CreateAsync(admin, adminPassword);
                if (!create.Succeeded)
                    throw new Exception(string.Join(", ", create.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, Roles.Administrator))
                await userManager.AddToRoleAsync(admin, Roles.Administrator);
        }
    }
}
