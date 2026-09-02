using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using TradeNest.Data.IntegrationTests.Utils;

namespace TradeNest.Data.IntegrationTests.RepositoriesTests;

public class CartsRepositoryTests : IntegrationTestsBase
{
    private CartsRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        this._repository = new CartsRepository(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        this._repository.Dispose();
    }

    [Test]
    public async Task GetUserCartWithProductsDetailsAsync_ShouldReturnCartWithProducts()
    {
        // Arrange
        ApplicationUser buyer = new ApplicationUser { Id = Guid.NewGuid() };
        (Product product, Category category, ApplicationUser owner)
            = TestDataGenerator.GetRandomProductWithOwnerAndCategory();
        Cart cart = new Cart { CartOwnerId = buyer.Id };
        CartProduct cartProduct = new CartProduct { Cart = cart, Product = product, ProductQuantityAdded = 1 };
        cart.CartProducts.Add(cartProduct);

        await SeedAsync(category);
        await SeedAsync(owner, buyer);
        await SeedAsync(product);
        await SeedAsync(cart);

        // Act
        Cart? result = await _repository.GetUserCartWithProductsDetailsAsync(buyer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.CartOwnerId, Is.EqualTo(buyer.Id));
        Assert.That(result.CartProducts.Count, Is.EqualTo(1));
        Assert.That(result.CartProducts.First().Product.Name, Is.EqualTo(product.Name));
    }
}