using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using static TradeNest.Tests.Common.RandomStringGenerator;

namespace TradeNest.Data.IntegrationTests.Utils;

internal static class TestDataGenerator
{
    internal static (Product product, Category category, ApplicationUser user) GetRandomProductWithOwnerAndCategory(
        Guid? productId = null,
        string? productName = null,
        ApprovalStatus approvalStatus = ApprovalStatus.Approved,
        ApplicationUser? user = null,
        Category? category = null)
    {
        user ??= new ApplicationUser() { Id = Guid.NewGuid(), UserName = RandomString(6, 15), };
        category ??= new Category() {  Id = Guid.NewGuid(), Name = RandomString(6, 15), };
        Product product = new Product()
        {
            Id = productId ?? Guid.NewGuid(),
            Name = productName ?? RandomString(6, 15),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = approvalStatus },
            Description = RandomString(10, 30),
            OwnerId = user.Id,
            CategoryId = category.Id,
        };
        
        return (product, category, user);
    }

    internal static (Product product, ApplicationUser user) GetRandomProductWithOwner(
        Category? category,
        Guid? productId = null,
        string? productName = null,
        ApprovalStatus approvalStatus = ApprovalStatus.Approved,
        ApplicationUser? user = null )
    {
        user ??= new ApplicationUser() { Id = Guid.NewGuid(), UserName = RandomString(6, 15), };
        category ??= new Category() {  Id = Guid.NewGuid(), Name = RandomString(6, 15), };
        Product product = new Product()
        {
            Id = productId ?? Guid.NewGuid(),
            Name = productName ?? RandomString(6, 15),
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = approvalStatus },
            Description = RandomString(10, 30),
            OwnerId = user.Id,
            CategoryId = category.Id,
        };
        
        return (product, user);
    }
}