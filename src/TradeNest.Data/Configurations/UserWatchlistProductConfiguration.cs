using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;

namespace TradeNest.Data.Configurations;

public class UserWatchlistProductConfiguration : IEntityTypeConfiguration<UserWatchlistProduct>
{
    public void Configure(EntityTypeBuilder<UserWatchlistProduct> builder)
    {
        builder.HasQueryFilter(wp => wp.Product.IsDeleted == false);
    }
}