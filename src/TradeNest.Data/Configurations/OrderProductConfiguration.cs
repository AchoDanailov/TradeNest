using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;

namespace TradeNest.Data.Configurations;

public class OrderProductConfiguration : IEntityTypeConfiguration<OrderProduct>
{
    public void Configure(EntityTypeBuilder<OrderProduct> builder)
    {
        builder.Property(op => op.TotalProductPriceAtOrderTime)
            .HasComputedColumnSql("[UnitSellingPriceAtOrderTime] * [QuantityOrdered]",
                stored: true)
            .ValueGeneratedOnAddOrUpdate();
        
        builder.HasOne(op => op.Order)
            .WithMany(o => o.OrderProducts)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(op => op.OriginalProduct)
            .WithMany(p => p.SoldProducts)
            .HasForeignKey(op => op.OriginalProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}