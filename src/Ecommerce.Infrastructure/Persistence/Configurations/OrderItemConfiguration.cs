using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(orderItem => orderItem.Id);

        builder.Property(orderItem => orderItem.ProductName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(orderItem => orderItem.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(orderItem => orderItem.Quantity)
            .IsRequired();

        builder.Property(orderItem => orderItem.CreatedAt)
            .IsRequired();

        builder.Property(orderItem => orderItem.CreatedBy)
            .HasMaxLength(100);

        builder.Property(orderItem => orderItem.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(orderItem => orderItem.DeletedBy)
            .HasMaxLength(100);

        builder.HasOne(orderItem => orderItem.Product)
            .WithMany()
            .HasForeignKey(orderItem => orderItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(orderItem => !orderItem.IsDeleted);
    }
}