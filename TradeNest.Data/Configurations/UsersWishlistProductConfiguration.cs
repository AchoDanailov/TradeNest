using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeNest.Data.Models;

namespace TradeNest.Data.Configurations;

public class UsersWishlistProductConfiguration 
    : IEntityTypeConfiguration<UsersWishlistProduct>
{
    public void Configure(EntityTypeBuilder<UsersWishlistProduct> builder)
    {
        builder.HasOne(w => w.User)
            .WithMany(u => u.WishlistProducts)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(w => w.Product)
            .WithMany(p => p.ProductsWishlists)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}