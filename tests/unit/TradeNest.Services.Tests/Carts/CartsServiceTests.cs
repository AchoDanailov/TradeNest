using Moq;
using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Cart;

namespace TradeNest.Services.Tests.Carts;

[TestFixture]
public class CartsServiceTests
{
    private Mock<ICartsRepository> _cartsRepositoryMock;
    private Mock<IUsersRepository> _usersRepositoryMock;
    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<ICartsMapper> _cartsMapperMock;
    private CartsService _cartsService;

    [SetUp]
    public void SetUp()
    {
        _cartsRepositoryMock = new Mock<ICartsRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _productsRepositoryMock = new Mock<IProductsRepository>();
        _cartsMapperMock = new Mock<ICartsMapper>();

        _cartsService = new CartsService(
            _cartsRepositoryMock.Object,
            _usersRepositoryMock.Object,
            _productsRepositoryMock.Object,
            _cartsMapperMock.Object);
    }

    [Test]
    public void GetCartByUserIdAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => 
            await _cartsService.GetCartByUserIdAsync(Guid.Empty));
    }

    [Test]
    public async Task GetCartByUserIdAsync_ShouldReturnNull_WhenCartIsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var cart = new Cart { Id = Guid.NewGuid(), CartProducts = new List<CartProduct>() };

        _usersRepositoryMock.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
        _cartsRepositoryMock.Setup(r => r.GetUserCartWithProductsDetailsAsync(userId, true)).ReturnsAsync(cart);

        // Act
        var result = await _cartsService.GetCartByUserIdAsync(userId);

        // Assert
        Assert.Null(result);
        _cartsRepositoryMock.Verify(r => r.DeleteAsync(cart), Times.Once);
    }

    [Test]
    public void AddProductToCartAsync_ShouldThrowInvalidOperationException_WhenOwnerTriesToAddToCart()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, OwnerId = userId };

        _usersRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>())).ReturnsAsync(true);
        _productsRepositoryMock.Setup(r => r.FindByIdAsync(productId)).ReturnsAsync(product);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _cartsService.AddProductToCartAsync(userId, productId, 1));
    }

    [Test]
    public void AddProductToCartAsync_ShouldThrowProductNotApprovedException_WhenProductNotApproved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product 
        { 
            Id = productId, 
            OwnerId = Guid.NewGuid(), 
            ApprovalDecision = new ApprovalDecision { ApprovalStatus = ApprovalStatus.Disapproved } 
        };

        _usersRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>())).ReturnsAsync(true);
        _productsRepositoryMock.Setup(r => r.FindByIdAsync(productId)).ReturnsAsync(product);

        // Act & Assert
        Assert.ThrowsAsync<ProductNotApprovedException>(async () => 
            await _cartsService.AddProductToCartAsync(userId, productId, 1));
    }

    [Test]
    public async Task AddProductToCartAsync_ShouldAddNewCart_WhenNoCartExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product 
        { 
            Id = productId, 
            OwnerId = Guid.NewGuid(), 
            ApprovalDecision = new ApprovalDecision { ApprovalStatus = ApprovalStatus.Approved },
            IsEnabled = true,
            QuantityInStock = 10
        };

        _usersRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>())).ReturnsAsync(true);
        _productsRepositoryMock.Setup(r => r.FindByIdAsync(productId)).ReturnsAsync(product);
        _cartsRepositoryMock.Setup(r => r.GetCartWithCartProductsByUserIdAsync(userId)).ReturnsAsync((Cart)null);
        _cartsRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Cart>())).ReturnsAsync(true);

        // Act
        await _cartsService.AddProductToCartAsync(userId, productId, 1);

        // Assert
        _cartsRepositoryMock.Verify(r => r.AddAsync(It.Is<Cart>(c => c.CartOwnerId == userId)), Times.Once);
    }
}