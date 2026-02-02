using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeNest.Data.Models;
using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Data.Configurations;

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql(DefaultValueForCreatedAtColumn);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(DefaultValueForIsActiveColumn);

        builder.HasQueryFilter(p => p.IsActive == true);
    }
}