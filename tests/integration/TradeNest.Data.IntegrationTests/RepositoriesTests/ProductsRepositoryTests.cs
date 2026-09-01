using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using TradeNest.Data.IntegrationTests.Utils;
using TradeNest.Data.Models.Enums;

namespace TradeNest.Data.IntegrationTests.RepositoriesTests;

public class ProductsRepositoryTests : IntegrationTestsBase
{
    private ProductsRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        this._repository = new ProductsRepository(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        this._repository.Dispose();
    }

    [Test]
    public async Task GetProductDetailsWithRelatedDataAsync_ShouldIncludeRelatedData()
    {
        // Arrange
        (Product product, Category category, ApplicationUser owner) 
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        
        await SeedAsync(category);
        await SeedAsync(owner);
        await SeedAsync(product);

        // Act
        Product? result = await _repository.GetProductDetailsWithRelatedDataAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Name, Is.EqualTo(product.Name));
        Assert.NotNull(result.Category);
        Assert.NotNull(result.Owner);
    }

    [TestCase(true, ApprovalStatus.Disapproved)]
    [TestCase(false, ApprovalStatus.Disapproved)]
    [TestCase(true, ApprovalStatus.Approved)]
    [TestCase(false, ApprovalStatus.Approved)]
    [TestCase(true, ApprovalStatus.WaitingApproval)]
    [TestCase(false, ApprovalStatus.WaitingApproval)]
    public async Task GetAllInclArchivedAndNotApprovedAsync_WithQueryOptions_ShouldWorkCorrectly(
        bool isDeleted,
        ApprovalStatus approvalStatus)
    {
        // Arrange
        (Product product, Category category, ApplicationUser owner)
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        (Product product2, Category category2, ApplicationUser owner2)
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        
        product.IsDeleted = isDeleted;
        product.ApprovalDecision.ApprovalStatus = approvalStatus;
        
        await SeedAsync(category, category2);
        await SeedAsync(owner, owner2);
        await SeedAsync(product, product2);
        
        // Act
        Product? products = (await this._repository
                .GetAllInclArchivedAndNotApprovedAsync(queryOptions =>
                    queryOptions.SetFilter(p => p.Id == product.Id)))
            .SingleOrDefault();
        
        // Assert
        Assert.NotNull(products);
    }
}