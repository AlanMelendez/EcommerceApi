using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(1000);

        builder.Property(product => product.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(product => product.Stock)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.Property(product => product.CreatedBy)
            .HasMaxLength(100);

        builder.Property(product => product.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(product => product.DeletedBy)
            .HasMaxLength(100);

        builder.HasIndex(product => product.Name);

        builder.HasQueryFilter(product => !product.IsDeleted);
    }
}