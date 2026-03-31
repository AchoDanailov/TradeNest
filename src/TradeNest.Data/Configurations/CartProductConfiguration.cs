using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using static TradeNest.Data.Common.EntityModelsConstants.CartProduct;

namespace TradeNest.Data.Configurations;

public class CartProductConfiguration : IEntityTypeConfiguration<CartProduct>
{
    public void Configure(EntityTypeBuilder<CartProduct> builder)
    {
        builder.HasOne(cp => cp.Cart)
            .WithMany(c => c.CartProducts)
            .HasForeignKey(cp => cp.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.Product)
            .WithMany(p => p.ProductCarts)
            .HasForeignKey(cp => cp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cp => cp.AddedOn)
            .HasDefaultValueSql(DefaultValueForAddedOnColumn);

        builder.HasQueryFilter(cp => cp.Product.IsDeleted == false &&
                                     cp.Product.ApprovalDecision.ApprovalStatus == ApprovalStatus.Approved);
    }
}