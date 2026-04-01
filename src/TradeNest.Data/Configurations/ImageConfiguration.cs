using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;

namespace TradeNest.Data.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasQueryFilter(i => i.Product.IsDeleted == false &&
                                    i.Product.ApprovalDecision.ApprovalStatus == ApprovalStatus.Approved);
    }
}