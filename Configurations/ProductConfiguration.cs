using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
            builder.Property(x => x.Description).IsRequired(false).HasMaxLength(1024);

            builder.Property(x => x.Price).IsRequired().HasPrecision(10, 2);

            builder.ToTable(options =>
            {
                options.HasCheckConstraint("CK_Products_Price", "[Price]>0");
            });

            builder.Property(x => x.SKU).IsRequired().HasMaxLength(64);
            builder.HasIndex(x => x.SKU).IsUnique();

            builder.Property(x => x.MainImageUrl).IsRequired().HasMaxLength(256);
            builder.Property(x => x.HoverImageUrl).IsRequired().HasMaxLength(256);

            builder.Property(x => x.CategoryId).IsRequired();
           
            builder.HasMany(x => x.ProductImages).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.ProductTags).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
