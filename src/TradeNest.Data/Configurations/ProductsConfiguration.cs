using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using static TradeNest.Data.Common.EntityModelsConstants.Product;

namespace TradeNest.Data.Configurations;

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasOne(p => p.Owner)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ApprovalDecisionMaker)
            .WithMany(a => a.ProductApprovalDecisionsGiven)
            .HasForeignKey(p => p.ApprovalDecisionMakerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.OwnsOne(p => p.ApprovalDecision);
        
        builder.Property(p => p.CreatedOn)
            .HasDefaultValueSql(DefaultValueForCreatedOnColumn);

        builder.Property(p => p.IsEnabled)
            .HasDefaultValue(DefaultValueForIsEnabledColumn);

        builder.HasQueryFilter(p => p.IsDeleted == false &&
                                    p.ApprovalDecision.ApprovalStatus == ApprovalStatus.Approved);
    }
}