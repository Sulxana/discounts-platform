using Discounts.Domain.Offers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastracture.Persistence.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.ToTable("Offer");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(1500);
            builder.Property(x => x.OriginalPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.DiscountedPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.TotalCoupons).IsRequired();
            builder.Property(x => x.RemainingCoupons).IsRequired();
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.DeletedAt).IsRequired(false);
            builder.Property(x => x.RejectionMessage).IsRequired(false);
            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.EndDate);
            builder.HasIndex(x => new { x.Status, x.Category, x.EndDate });

        }
    }
}
