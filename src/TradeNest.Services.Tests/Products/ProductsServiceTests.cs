using Moq;
using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Product;

namespace TradeNest.Services.Tests.Products;

[TestFixture]
public class ProductsServiceTests
{
    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<IUsersRepository> _usersRepositoryMock;
    private Mock<ICategoriesRepository> _categoriesRepositoryMock;
    private Mock<IAdminsRepository> _adminsRepositoryMock;
    private Mock<IProductsMapper> _productsMapperMock;
    private ProductsService _productsService;

    [SetUp]
    public void SetUp()
    {
        _productsRepositoryMock = new Mock<IProductsRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _categoriesRepositoryMock = new Mock<ICategoriesRepository>();
        _adminsRepositoryMock = new Mock<IAdminsRepository>();
        _productsMapperMock = new Mock<IProductsMapper>();

        _productsService = new ProductsService(
            _productsRepositoryMock.Object,
            _usersRepositoryMock.Object,
            _categoriesRepositoryMock.Object,
            _adminsRepositoryMock.Object,
            _productsMapperMock.Object);
    }

    [Test]
    public async Task GetProductDetailsByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productsRepositoryMock.Setup(r => r.GetProductDetailsWithRelatedDataAsync(productId, true)).ReturnsAsync((Product)null);

        // Act
        var result = await _productsService.GetProductDetailsByIdAsync(productId, Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Test]
    public async Task GetProductDetailsByIdAsync_ShouldReturnProductDetails_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product 
        { 
            Id = productId, 
            OwnerId = userId, 
            ApprovalDecision = new ApprovalDecision { ApprovalStatus = ApprovalStatus.Approved } 
        };
        var productDetailsDto = new ProductDetailsDto { Id = productId };

        _productsRepositoryMock.Setup(r => r.GetProductDetailsWithRelatedDataAsync(productId, true)).ReturnsAsync(product);
        _productsMapperMock.Setup(m => m.ToProductDetailsDto(product, true)).Returns(productDetailsDto);

        // Act
        var result = await _productsService.GetProductDetailsByIdAsync(productId, userId);

        // Assert
        Assert.That(result, Is.EqualTo(productDetailsDto));
    }

    [Test]
    public void ChangeProductApprovalStatus_ShouldThrowUnauthorizedOperationException_WhenUserIsNotAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _adminsRepositoryMock.Setup(r => r.IsUserAdminByUserIdAsync(userId)).ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedOperationException>(async () => 
            await _productsService.ChangeProductApprovalStatus(userId, Guid.NewGuid(), new EditApprovalDecisionDto()));
    }

    [Test]
    public async Task ChangeProductApprovalStatus_ShouldUpdateProduct_WhenUserIsAdmin()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, ApprovalDecision = new ApprovalDecision() };
        var dto = new EditApprovalDecisionDto { ApprovalStatus = TradeNest.Services.Models.Enums.ApprovalStatus.Approved };
        var admin = new Admin { Id = Guid.NewGuid() };

        _adminsRepositoryMock.Setup(r => r.IsUserAdminByUserIdAsync(adminId)).ReturnsAsync(true);
        _adminsRepositoryMock.Setup(r => r.GetAdminByUserId(adminId)).ReturnsAsync(admin);
        
        _productsRepositoryMock.Setup(r => r.GetAllInclNotApprovedAsync(It.IsAny<Action<IQueryOptions<Product>>>()))
            .ReturnsAsync(new List<Product> { product });
        
        _productsRepositoryMock.Setup(r => r.UpdateAsync(product)).ReturnsAsync(true);

        // Act
        await _productsService.ChangeProductApprovalStatus(adminId, productId, dto);

        // Assert
        _productsRepositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
        Assert.That(product.ApprovalDecision.ApprovalStatus, Is.EqualTo(ApprovalStatus.Approved));
    }
}