using Discounts.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastracture.Persistence.Configurations
{
    public class GlobalSettingConfiguration : IEntityTypeConfiguration<GlobalSetting>
    {
        public void Configure(EntityTypeBuilder<GlobalSetting> builder)
        {
            builder.ToTable("GlobalSetting");

            builder.HasKey(s => s.Key);
            builder.Property(s => s.Key).HasMaxLength(100).IsRequired();
            builder.Property(s => s.Value).HasMaxLength(500).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(500).IsRequired();
            builder.Property(s => s.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(s => s.UpdatedAt).IsRequired();

            builder.HasData(
                new GlobalSetting("Reservation.ExpirationMinutes", "30", "Minutes before reservation expires", SettingType.Integer),
                new GlobalSetting("Reservation.MaxQuantity", "10", "Maximum quantity per reservation", SettingType.Integer),
                new GlobalSetting("Merchant.EditWindowHours", "24", "Hours after creation when merchant can edit offer", SettingType.Integer)
            );
        }
    }
}
