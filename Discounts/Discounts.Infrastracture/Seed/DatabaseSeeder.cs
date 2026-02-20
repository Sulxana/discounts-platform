using Discounts.Application.Common.Security;
using Discounts.Domain.Categories;
using Discounts.Domain.Offers;
using Discounts.Infrastracture.Identity;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(DiscountsDbContext context, UserManager<ApplicationUser> userManager)
        {
            var merchant = await userManager.FindByEmailAsync("merchant@example.com");
            if (merchant == null) return;

            if (!await context.Set<Category>().AnyAsync())
            {
                var technology = new Category("Technology");
                var food = new Category("Food & Dining");
                var entertainment = new Category("Entertainment");

                context.Set<Category>().AddRange(technology, food, entertainment);
                await context.SaveChangesAsync();

                if (!await context.Set<Offer>().AnyAsync())
                {
                    var offer1 = new Offer(
                        "50% Off Laptops",
                        "Get half off selected models.",
                        technology.Id,
                        "https://example.com/laptop.jpg",
                        1000m,
                        500m,
                        50,
                        DateTime.UtcNow.AddDays(-1),
                        DateTime.UtcNow.AddDays(7),
                        merchant.Id
                    );
                    offer1.Approve();

                    var offer2 = new Offer(
                        "Free Burger",
                        "Buy one get one free.",
                        food.Id,
                        "https://example.com/burger.jpg",
                        15m,
                        7.5m,
                        100,
                        DateTime.UtcNow.AddDays(-1),
                        DateTime.UtcNow.AddDays(30),
                        merchant.Id
                    );
                    offer2.Approve();

                    context.Set<Offer>().AddRange(offer1, offer2);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
