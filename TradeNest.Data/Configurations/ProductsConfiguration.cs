using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeNest.Data.Models;
using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Data.Configurations;

//TODO: Add Soft Deletion
public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasOne(p => p.Owner)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql(DefaultValueForCreatedAtColumn);

        builder.Property(p => p.IsEnabled)
            .HasDefaultValue(DefaultValueForIsEnabledColumn);
    }
}