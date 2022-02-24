using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrastructure.Data.Config;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(order => order.ShipToAddress, address =>
        {
            address.WithOwner();
        });

        builder.Property(order => order.Status)
            .HasConversion(
                statusEnum => statusEnum.ToString(),
                statusString => (OrderStatus)Enum.Parse(typeof(OrderStatus), statusString)
            );

        builder.HasMany(order => order.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
