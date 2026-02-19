using Discounts.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infrastracture.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasData(
                new Category("Restaurant") { Id = Guid.Parse("11111111-1111-1111-1111-111111111111") },
                new Category("Fitness") { Id = Guid.Parse("22222222-2222-2222-2222-222222222222") },
                new Category("Tourism") { Id = Guid.Parse("33333333-3333-3333-3333-333333333333") },
                new Category("Entertainment") { Id = Guid.Parse("44444444-4444-4444-4444-444444444444") },
                new Category("Services") { Id = Guid.Parse("55555555-5555-5555-5555-555555555555") },
                new Category("Education") { Id = Guid.Parse("66666666-6666-6666-6666-666666666666") },
                new Category("Other") { Id = Guid.Parse("77777777-7777-7777-7777-777777777777") }
            );
        }
    }
}
