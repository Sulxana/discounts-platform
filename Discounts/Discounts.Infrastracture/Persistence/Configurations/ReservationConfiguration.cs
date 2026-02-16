using Discounts.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastracture.Persistence.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.OfferId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasConversion<string>();

            builder.HasIndex(x => new { x.UserId, x.OfferId }); // user-ის აქტიური რეზერვები
            builder.HasIndex(x => new { x.Status, x.ExpiresAt });//user-ის expired აქტიური რეზერვები
        }
    }
}
