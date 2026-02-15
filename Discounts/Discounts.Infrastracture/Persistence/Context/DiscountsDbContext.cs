using Discounts.Domain.Offers;
using Discounts.Domain.Auth;
using Discounts.Infrastracture.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Persistence.Context
{
    //public class DiscountsDbContext : DbContext
    public class DiscountsDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DiscountsDbContext(DbContextOptions<DiscountsDbContext> options) : base(options)
        {

        }
        public DbSet<RefreshToken> RefreshTokens{ get; set; }


        #region Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Offer>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountsDbContext).Assembly);
        }

        #endregion
    }
}
