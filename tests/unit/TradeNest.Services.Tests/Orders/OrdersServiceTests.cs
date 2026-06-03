using Moq;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Order;

namespace TradeNest.Services.Tests.Orders;

[TestFixture]
public class OrdersServiceTests
{
    private Mock<IOrdersRepository> _ordersRepositoryMock;
    private Mock<IUsersRepository> _usersRepositoryMock;
    private Mock<ICartsRepository> _cartsRepositoryMock;
    private Mock<IOrdersMapper> _ordersMapperMock;
    private OrdersService _ordersService;

    [SetUp]
    public void SetUp()
    {
        _ordersRepositoryMock = new Mock<IOrdersRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _cartsRepositoryMock = new Mock<ICartsRepository>();
        _ordersMapperMock = new Mock<IOrdersMapper>();

        _ordersService = new OrdersService(
            _ordersRepositoryMock.Object,
            _usersRepositoryMock.Object,
            _cartsRepositoryMock.Object,
            _ordersMapperMock.Object);
    }

    [Test]
    public void GetAllOrdersByUserIdAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => 
            await _ordersService.GetAllOrdersByUserIdAsync(Guid.Empty));
    }

    [Test]
    public async Task GetAllOrdersByUserIdAsync_ShouldReturnOrders_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orders = new List<Order> { new Order { Id = Guid.NewGuid(), UserId = userId } };
        var orderDtos = new List<OrderDto> { new OrderDto { Id = orders[0].Id } };

        _usersRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync(true);

        _ordersRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<Action<IQueryOptions<Order>>>()))
            .ReturnsAsync(orders);

        _ordersMapperMock
            .Setup(m => m.ToOrderDtos(orders))
            .Returns(orderDtos);

        // Act
        var result = await _ordersService.GetAllOrdersByUserIdAsync(userId);

        // Assert
        Assert.That(result, Is.EqualTo(orderDtos));
    }

    [Test]
    public async Task SubmitOrderAsync_ShouldReturnSuccess_WhenOrderIsSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cart = new Cart 
        { 
            Id = Guid.NewGuid(), 
            CartProducts = new List<CartProduct> 
            { 
                new CartProduct 
                { 
                    ProductId = Guid.NewGuid(), 
                    ProductQuantityAdded = 1,
                    Product = new Product { IsEnabled = true, QuantityInStock = 10, SellingPrice = 100 }
                } 
            } 
        };

        _usersRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync(true);

        _cartsRepositoryMock
            .Setup(r => r.GetUserCartWithProductsDetailsAsync(userId, false))
            .ReturnsAsync(cart);

        _ordersMapperMock
            .Setup(m => m.OrderProductFromCartProduct(It.IsAny<CartProduct>()))
            .Returns(new OrderProduct());

        _ordersRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(true);

        // Act
        var result = await _ordersService.SubmitOrderAsync(userId);

        // Assert
        Assert.True(result.IsSuccess);
        _ordersRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    [Test]
    public async Task SubmitOrderAsync_ShouldReturnFailure_WhenProductIsDisabled()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cart = new Cart 
        { 
            Id = Guid.NewGuid(), 
            CartProducts = new List<CartProduct> 
            { 
                new CartProduct 
                { 
                    ProductId = Guid.NewGuid(), 
                    ProductQuantityAdded = 1,
                    Product = new Product { Name = "Test Product", IsEnabled = false, QuantityInStock = 10 }
                } 
            } 
        };

        _usersRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync(true);

        _cartsRepositoryMock
            .Setup(r => r.GetUserCartWithProductsDetailsAsync(userId, false))
            .ReturnsAsync(cart);

        // Act
        var result = await _ordersService.SubmitOrderAsync(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.That(result.ErrorProducts.Count, Is.EqualTo(1));
        Assert.That(result.ErrorProducts.First().ProductName, Is.EqualTo("Test Product"));
    }
}