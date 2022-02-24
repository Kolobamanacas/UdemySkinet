using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(product => product.Id).IsRequired();
        builder.Property(product => product.Name).IsRequired().HasMaxLength(100);
        builder.Property(product => product.Name).IsRequired();
        builder.Property(product => product.Price).HasColumnType("decimal(18,2)");
        builder.Property(product => product.PictureUrl).IsRequired();
        builder.HasOne(product => product.ProductBrand).WithMany().HasForeignKey(product => product.ProductBrandId);
        builder.HasOne(product => product.ProductType).WithMany().HasForeignKey(product => product.ProductTypeId);
    }
}
