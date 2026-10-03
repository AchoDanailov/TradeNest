using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;

namespace TradeNest.Data.Tests.RepositoriesTests;

public class OrdersRepositoryTests : BaseRepositoriesTest
{
    private OrdersRepository _ordersRepository;

    [SetUp]
    public async Task SetUpOrdersRepository()
    {
        this._ordersRepository = new OrdersRepository(this.DbContext);
    }

    [TearDown]
    public async Task TearDownOrdersRepository()
    {
        this._ordersRepository.Dispose();
    }

    [Test]
    public async Task AddAsync_WorksCorrectly()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser() { Id = Guid.NewGuid() };
        await this.SeedAsync(user);

        Cart cart = new Cart() { Id = Guid.NewGuid(), CartOwnerId = user.Id };
        await this.SeedAsync(cart);

        Order order = new Order()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SubmittedOn = DateTime.UtcNow,
        };

        // Act
        bool isAdded = await this._ordersRepository.AddAsync(order);
        Order? fetchedOrder = await this.DbContext.Orders.FindAsync(order.Id);
        Cart? fetchedCart = await this.DbContext.Carts.FindAsync(cart.Id);

        // Assert
        Assert.That(isAdded, Is.True);
        Assert.That(fetchedOrder, Is.Not.Null);
        Assert.That(fetchedCart, Is.Null);
    }
}