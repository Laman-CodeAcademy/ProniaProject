﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.Property(b => b.Count).IsRequired();

            builder.HasOne(b => b.Product)
                   .WithMany(u => u.Baskets)
                   .HasForeignKey(b => b.ProductId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.HasOne(b => b.AppUser)
                     .WithMany(u => u.Baskets)
                     .HasForeignKey(b => b.AppUserId)
                     .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.ToTable(opt =>
            {
                opt.HasCheckConstraint("CK_Basket_Count_NonNegative", "[Count] >= 0");
            });



        }
    }
}