using NUnit.Framework;
using TradeNest.Data.Models;
using TradeNest.Data.Repository;

namespace TradeNest.Data.IntegrationTests.Repositories;

[TestFixture]
public class CartsRepositoryTests : RepositoryTestsBase
{
    private CartsRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new CartsRepository(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
    }

    [Test]
    public async Task GetUserCartWithProductsDetailsAsync_ShouldReturnCartWithProducts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Category" };
        var product = new Product 
        { 
            Name = "Product", 
            OwnerId = Guid.NewGuid(), 
            Category = category,
            Description = "Desc",
            SellingPrice = 10,
            QuantityInStock = 5,
            RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 },
            ApprovalDecision = new ApprovalDecision { ApprovalStatus = TradeNest.Data.Models.Enums.ApprovalStatus.Approved }
        };
        var cart = new Cart { CartOwnerId = userId };
        var cartProduct = new CartProduct { Cart = cart, Product = product, ProductQuantityAdded = 1 };
        cart.CartProducts.Add(cartProduct);

        await SeedAsync(category);
        await SeedAsync(product);
        await SeedAsync(cart);

        // Act
        var result = await _repository.GetUserCartWithProductsDetailsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.CartOwnerId, Is.EqualTo(userId));
        Assert.That(result.CartProducts.Count, Is.EqualTo(1));
        Assert.That(result.CartProducts.First().Product.Name, Is.EqualTo("Product"));
    }
}