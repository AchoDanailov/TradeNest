using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;

namespace TradeNest.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasOne(c => c.CartOwner)
            .WithOne(u => u.Cart)
            .HasForeignKey<Cart>(c => c.CartOwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}