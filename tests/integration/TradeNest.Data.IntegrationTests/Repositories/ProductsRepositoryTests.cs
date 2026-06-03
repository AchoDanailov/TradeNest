using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;

namespace TradeNest.Data.IntegrationTests.Repositories;

[TestFixture]
public class ProductsRepositoryTests : RepositoryTestsBase
{
    private ProductsRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new ProductsRepository(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
    }

    [Test]
    public async Task GetProductDetailsWithRelatedDataAsync_ShouldIncludeRelatedData()
    {
        // Arrange
        var category = new Category { Name = "Cat" };
        var owner = new ApplicationUser { UserName = "owner", Email = "owner@test.com" };
        var product = new Product 
        { 
            Name = "Detailed Product", 
            Category = category, 
            Owner = owner,
            Description = "Desc",
            SellingPrice = 10,
            QuantityInStock = 5,
            RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 },
            ApprovalDecision = new ApprovalDecision { ApprovalStatus = TradeNest.Data.Models.Enums.ApprovalStatus.Approved }
        };

        await SeedAsync(category);
        await SeedAsync(owner);
        await SeedAsync(product);

        // Act
        var result = await _repository.GetProductDetailsWithRelatedDataAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Name, Is.EqualTo("Detailed Product"));
        Assert.NotNull(result.Category);
        Assert.NotNull(result.Owner);
    }
}