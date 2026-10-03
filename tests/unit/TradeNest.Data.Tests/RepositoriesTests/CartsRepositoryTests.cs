using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using TradeNest.Data.Repository;

namespace TradeNest.Data.Tests.RepositoriesTests;

public class CartsRepositoryTests : BaseRepositoriesTest
{
    private CartsRepository _cartsRepository;

    [SetUp]
    public async Task SetUpCartsRepository()
    {
        this._cartsRepository = new CartsRepository(this.DbContext);
    }

    [TearDown]
    public async Task TearDownCartsRepository()
    {
        this._cartsRepository.Dispose();
    }

    [Test]
    public async Task GetCartWithProductsDetailsAsync_WhenCartDoesNotExist_ReturnsNull()
    {
        // Act
        Cart? cart = await this._cartsRepository.GetCartWithProductsDetailsAsync(Guid.NewGuid());

        // Assert
        Assert.That(cart, Is.Null);
    }

    [Test]
    public async Task GetCartWithProductsDetailsAsync_WorksCorrectly()
    {
        // Arrange
        (ApplicationUser user, Cart cart, Product product, CartProduct cartProduct)
            = await this.SeedCartWithProductAsync();
        this.DbContext.ChangeTracker.Clear();

        // Act
        Cart? fetchedCart = await this._cartsRepository.GetCartWithProductsDetailsAsync(cart.Id);

        // Assert
        Assert.That(fetchedCart, Is.Not.Null);
        Assert.That(fetchedCart.CartProducts.Single().Product.Id, Is.EqualTo(product.Id));
        Assert.That(fetchedCart.CartProducts.Single().Product.Name, Is.EqualTo(product.Name));
    }

    [Test]
    public async Task GetUserCartWithProductsDetailsAsync_WhenCartDoesNotExist_ReturnsNull()
    {
        // Act
        Cart? cart = await this._cartsRepository
            .GetUserCartWithProductsDetailsAsync(Guid.NewGuid());

        // Assert
        Assert.That(cart, Is.Null);
    }

    [Test]
    public async Task GetUserCartWithProductsDetailsAsync_AsReadOnly_ReturnsCartWithoutTracking()
    {
        // Arrange
        (ApplicationUser user, Cart cart, Product product, CartProduct cartProduct)
            = await this.SeedCartWithProductAsync();
        this.DbContext.ChangeTracker.Clear();

        // Act
        Cart? fetchedCart = await this._cartsRepository
            .GetUserCartWithProductsDetailsAsync(user.Id, asReadOnly: true);

        // Assert
        Assert.That(fetchedCart, Is.Not.Null);
        Assert.That(fetchedCart.CartProducts.Single().Product.Id, Is.EqualTo(product.Id));
        Assert.That(this.DbContext.Entry(fetchedCart).State, Is.EqualTo(EntityState.Detached));
    }

    [Test]
    public async Task GetCartWithCartProductsByUserIdAsync_WhenCartDoesNotExist_ReturnsNull()
    {
        // Act
        Cart? cart = await this._cartsRepository.GetCartWithCartProductsByUserIdAsync(Guid.NewGuid());

        // Assert
        Assert.That(cart, Is.Null);
    }

    [Test]
    public async Task GetCartWithCartProductsByUserIdAsync_WorksCorrectly()
    {
        // Arrange
        (ApplicationUser user, Cart cart, Product product, CartProduct cartProduct)
            = await this.SeedCartWithProductAsync();
        this.DbContext.ChangeTracker.Clear();

        // Act
        Cart? fetchedCart = await this._cartsRepository
            .GetCartWithCartProductsByUserIdAsync(user.Id);

        // Assert
        Assert.That(fetchedCart, Is.Not.Null);
        Assert.That(fetchedCart.CartProducts.Single().ProductId, Is.EqualTo(product.Id));
    }

    [Test]
    public async Task AddAsync_WorksCorrectly()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid() };
        await this.SeedAsync(user);
        Cart cart = new Cart() { Id = Guid.NewGuid(), CartOwnerId = user.Id };

        // Act
        bool isAdded = await this._cartsRepository.AddAsync(cart);
        Cart? fetchedCart = await this.DbContext.Carts.FindAsync(cart.Id);

        // Assert
        Assert.That(isAdded, Is.True);
        Assert.That(fetchedCart, Is.Not.Null);
    }

    [Test]
    public async Task AddRangeAsync_WorksCorrectly()
    {
        // Arrange
        ApplicationUser firstUser = new ApplicationUser() { Id = Guid.NewGuid() };
        ApplicationUser secondUser = new ApplicationUser() { Id = Guid.NewGuid() };
        await this.SeedAsync(firstUser, secondUser);
        Cart firstCart = new Cart() { Id = Guid.NewGuid(), CartOwnerId = firstUser.Id };
        Cart secondCart = new Cart() { Id = Guid.NewGuid(), CartOwnerId = secondUser.Id };

        // Act
        bool isAdded = await this._cartsRepository.AddRangeAsync(new[] { firstCart, secondCart });

        // Assert
        Assert.That(isAdded, Is.True);
        Assert.That(await this.DbContext.Carts.AnyAsync(c => c.Id == firstCart.Id), Is.True);
        Assert.That(await this.DbContext.Carts.AnyAsync(c => c.Id == secondCart.Id), Is.True);
    }

    [Test]
    public async Task UpdateAsync_WhenCartHasProducts_SavesChanges()
    {
        // Arrange
        (ApplicationUser user, Cart cart, Product product, CartProduct cartProduct)
            = await this.SeedCartWithProductAsync();
        cartProduct.ProductQuantityAdded = 3;

        // Act
        bool isUpdated = await this._cartsRepository.UpdateAsync(cart);
        this.DbContext.ChangeTracker.Clear();
        CartProduct? fetchedCartProduct = await this.DbContext.CartsProducts
            .SingleOrDefaultAsync(cp => cp.CartId == cart.Id && cp.ProductId == product.Id);

        // Assert
        Assert.That(isUpdated, Is.True);
        Assert.That(fetchedCartProduct!.ProductQuantityAdded, Is.EqualTo(3));
    }

    [Test]
    public async Task DeleteAsync_WorksCorrectly()
    {
        // Arrange
        (ApplicationUser user, Cart cart, Product product, CartProduct cartProduct)
            = await this.SeedCartWithProductAsync();

        // Act
        bool isDeleted = await this._cartsRepository.DeleteAsync(cart);
        Cart? fetchedCart = await this.DbContext.Carts.FindAsync(cart.Id);

        // Assert
        Assert.That(isDeleted, Is.True);
        Assert.That(fetchedCart, Is.Null);
    }

    private async Task<(ApplicationUser User, Cart Cart, Product Product, CartProduct CartProduct)>
        SeedCartWithProductAsync()
    {
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid() };
        Category category = new Category() { Id = Guid.NewGuid() };
        Product product = new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Test product",
            Description = "Test product description",
            SellingPrice = 10m,
            CategoryId = category.Id,
            OwnerId = user.Id,
            ApprovalDecision = new ApprovalDecision() { ApprovalStatus = ApprovalStatus.Approved },
            CreatedOn = DateTime.UtcNow,
        };
        Cart cart = new Cart() { Id = Guid.NewGuid(), CartOwnerId = user.Id };
        CartProduct cartProduct = new CartProduct()
        {
            CartId = cart.Id,
            ProductId = product.Id,
            ProductQuantityAdded = 1,
            AddedOn = DateTime.UtcNow,
        };

        await this.SeedAsync(user);
        await this.SeedAsync(category);
        await this.SeedAsync(product);
        await this.SeedAsync(cart);
        await this.SeedAsync(cartProduct);

        return (user, cart, product, cartProduct);
    }
}
