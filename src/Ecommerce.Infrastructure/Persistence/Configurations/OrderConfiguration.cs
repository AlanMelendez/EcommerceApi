using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.CustomerId)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasConversion<int>() //Ef core convert the property to int when storing in the DB
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.CreatedBy)
            .HasMaxLength(100);

        builder.Property(order => order.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(order => order.DeletedBy)
            .HasMaxLength(100);

        builder.HasMany(order => order.Items)
            .WithOne(orderItem => orderItem.Order)
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(order => !order.IsDeleted);
    }
}