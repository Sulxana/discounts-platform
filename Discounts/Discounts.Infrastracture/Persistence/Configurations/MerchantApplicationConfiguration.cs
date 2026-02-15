using Discounts.Domain.MerchantApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastracture.Persistence.Configurations
{
    public class MerchantApplicationConfiguration : IEntityTypeConfiguration<MerchantApplication>
    {
        public void Configure(EntityTypeBuilder<MerchantApplication> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.RejectionReason)
                .HasMaxLength(500);
        }
    }
}
