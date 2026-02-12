using Discounts.Domain.Offers;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Persistence.Context
{
    public class DiscountsDbContext : DbContext
    {
        public DiscountsDbContext(DbContextOptions<DiscountsDbContext> options) : base(options)
        {

        }

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
