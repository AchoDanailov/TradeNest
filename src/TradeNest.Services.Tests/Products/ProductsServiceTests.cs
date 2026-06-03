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

    private IEnumerable<Product> _stdProducts; 

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

        this._stdProducts = SetupProductsCollection();
    }

    [TestCase(null)] [TestCase("tv")] [TestCase("plane")]
    public async Task GetAllProductsOrderedByDateOfCreationDescAsync_WorksCorrectly(
        string? searchString)
    {
        // Arrange
        this._productsRepositoryMock
            .Setup(pr => pr.GetAllProductsWithCategoryAndImagesAsync(It.IsAny<Action<IQueryOptions<Product>>>()))
            .ReturnsAsync(() =>
            {
                IEnumerable<Product> products = new List<Product>()
                {
                    new Product() { Id = Guid.NewGuid(), Name = "Big TV", },
                    new Product() { Id = Guid.NewGuid(), Name = "Basketball", },
                };

                if (searchString != null)
                {
                    return products
                        .Where(p => p.Name
                            .Contains(searchString, StringComparison.InvariantCultureIgnoreCase));
                }

                return products;
            });

        this._productsMapperMock
            .Setup(pm => pm.ToProductDtos(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> products) =>
            {
                return products.Select(p => new ProductDto()
                {
                    Id = p.Id,
                    Name = p.Name
                });
            });

        // Act
        IEnumerable<ProductDto> products = (await this._productsService
                .GetAllProductsOrderedByDateOfCreationDescAsync(searchString))
            .ToArray();
        
        // Assert
        if (searchString == null)
        {
            Assert.That(products.Count(), Is.EqualTo(2));
        }
        else if (searchString == "tv")
        {
            Assert.That(products.Count(), Is.EqualTo(1));
            Assert.That(products.First().Name.ToLowerInvariant(), Does.Contain("tv"));
        }
        else
        {
            Assert.That(products.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public void GetAllProductsByCategoryIdAsync_OnEmptyCategoryId_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this._productsService.GetAllProductsByCategoryIdAsync(Guid.Empty));
    }

    [TestCase("tv")] [TestCase("plane")]
    public async Task GetAllProductsByCategoryIdAsync_WithSearchQuery_WorksCorrectly(string searchQuery)
    {
        // Arrange
        ((Guid CategoryId, string CategoryName) category, IEnumerable<Product> products) arrangedData
            = this.ArrangeForGetAllProductsByCategoryIdAsyncTests();
        
        this._productsRepositoryMock
            .Setup(pr => pr.GetAllProductsWithCategoryAndImagesAsync(It.IsAny<Action<IQueryOptions<Product>>>()))
            .ReturnsAsync(arrangedData.products
                .Where(p => p.CategoryId == arrangedData.category.CategoryId &&
                            (p.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()) ||
                             p.Category.Name.ToLowerInvariant().Contains(searchQuery.ToLowerInvariant()))));
        
        // Act
        IEnumerable<ProductDto> productDtos = (await this._productsService
                .GetAllProductsByCategoryIdAsync(arrangedData.category.CategoryId, searchQuery))
            .ToArray();

        // Assert
        if (searchQuery == "plane")
            Assert.That(productDtos, Is.Empty);
        
        string query = searchQuery.ToLowerInvariant();
        if (productDtos.Any())
        {
            Assert.That(productDtos, Has.All.Matches<ProductDto>(p =>
                (p.Name.ToLowerInvariant().Contains(query) ||
                 p.CategoryName.ToLowerInvariant().Contains(query)) &&
                p.CategoryName == arrangedData.category.CategoryName));
        }
    }

    [Test]
    public async Task GetAllProductsByCategoryIdAsync_WithoutSearchQuery_WorksCorrectly()
    {
        // Arrange
        ((Guid CategoryId, string CategoryName) category, IEnumerable<Product> products) arrangedData
            = ArrangeForGetAllProductsByCategoryIdAsyncTests();
        
        this._productsRepositoryMock
            .Setup(pr => pr.GetAllProductsWithCategoryAndImagesAsync(It.IsAny<Action<IQueryOptions<Product>>>()))
            .ReturnsAsync(arrangedData.products
                .Where(p => p.CategoryId == arrangedData.category.CategoryId));
        
        // Act
        IEnumerable<ProductDto> productDtos = (await this._productsService
                .GetAllProductsByCategoryIdAsync(arrangedData.category.CategoryId))
            .ToArray();
        
        // Assert
        Assert.That(productDtos.Count(), Is.EqualTo(3));
        Assert.That(
            productDtos.Select(p => p.CategoryName),
            Is.All.EqualTo(arrangedData.category.CategoryName));
    }

    private ValueTuple<ValueTuple<Guid, string>, IEnumerable<Product>>
        ArrangeForGetAllProductsByCategoryIdAsyncTests(string? categoryName = null)
    {
        categoryName ??= "TargetCategory";
        
        ValueTuple<Guid, string> categoryIdWithCategoryName 
            = new ValueTuple<Guid, string>(Guid.Parse("11111111-1111-1111-1111-111111111111"), categoryName);

        IEnumerable<Product> products = SetupProductsCollection(null, categoryIdWithCategoryName);
        
        this._productsMapperMock
            .Setup(pm => pm.ToProductDtos(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> filteredProds) =>
            {
                return filteredProds.Select(p => new ProductDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name
                });
            });

        return (categoryIdWithCategoryName, products);
    }

    [Test]
    public async Task GetAllProductsOrderedBySellingCountDescAsync_WorksCorrectly()
    {
        // Arrange
        this._productsRepositoryMock
            .Setup(pr => pr.GetAllProductsWithCategoryAndImagesAsync(It.IsAny<Action<IQueryOptions<Product>>>()))
            .ReturnsAsync(() =>
            {
                return this._stdProducts.OrderByDescending(p => p.SellingPrice);
            });

        this._productsMapperMock
            .Setup(pm => pm.ToProductDtos(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> filteredProds) => filteredProds.Select(p => new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name
            }));
        
        // Act
        IEnumerable<ProductDto> productDtos = (await this._productsService
                .GetAllProductsOrderedBySellingCountDescAsync())
            .ToArray();
        
        // Assert
        Assert.That(productDtos.Count(), Is.GreaterThan(0));
        
        bool firstProdIsMostExpensive = productDtos.Skip(1)
            .All(p => p.SellingPrice <= productDtos.First().SellingPrice);
        Assert.That(firstProdIsMostExpensive, Is.True);
    }

    [Test]
    public async Task ProductExistsByIdAsync_OnPassedEmptyGuid_ReturnsFalse()
    {
        bool actual = await this._productsService.ProductExistsByIdAsync(Guid.Empty);
        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task ProductExistsByIdAsync_WorksCorrectly()
    {
        // Arrange 
        Guid productId = Guid.NewGuid();
        IEnumerable<Product> products = SetupProductsCollection(productId);

        this._productsRepositoryMock
            .Setup(pr => pr.ExistsAsync(p => p.Id == productId))
            .ReturnsAsync(products.Any(p => p.Id == productId));
        
        // Act
        bool actual = await this._productsService.ProductExistsByIdAsync(productId);
        
        // Assert
        Assert.That(actual, Is.True);
    }

    [Test]
    public void GetSpecifiedProductsCountAsync_OnPassedUserIdEmptyGuid_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await this._productsService.GetSpecifiedProductsCountAsync(Guid.Empty));
    }

    [Test]
    public void GetSpecifiedProductsCountAsync_WhenNotAdmin_ThrowsUnauthorizedOperationException()
    {
        // Arrange
        Guid notUserAdminId = Guid.NewGuid();
        this._adminsRepositoryMock
            .Setup(ar => ar.IsUserAdminByUserIdAsync(notUserAdminId))
            .ReturnsAsync(false);
        
        Assert.ThrowsAsync<UnauthorizedOperationException>(async () =>
            await this._productsService.GetSpecifiedProductsCountAsync(notUserAdminId));
    }

    [TestCase(null, null)]
    [TestCase(true, null)]
    [TestCase(false, null)]
    [TestCase(true, "tv")]
    [TestCase(false, "tv")]
    public async Task GetSpecifiedProductsCountAsync_WorksCorrectly(bool? approved, string? searchQuery)
    {
        // Arrange
        IEnumerable<Product> products = SetupProductsCollection(
            productId: null,
            categoryIdWithCategoryName: null,
            withSomeDisapproved: true);
        
        this._adminsRepositoryMock
            .Setup(ar => ar.IsUserAdminByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        
        this._productsRepositoryMock
            .Setup(pr => pr.GetSpecifiedProductsCount(
                It.IsAny<bool?>(),
                It.IsAny<string?>()))
            .ReturnsAsync(() =>
            {
                IEnumerable<Product> ret = products;

                if (approved is false)
                    ret = ret.Where(p => p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved);
                else if (approved is true)
                    ret = ret.Where(p => p.ApprovalDecision.ApprovalStatus == ApprovalStatus.Approved);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                    ret = ret.Where(p => p.Name.ToLower().Contains(searchQuery.ToLower()) ||
                                         p.Category.Name.ToLower().Contains(searchQuery.ToLower()));

                return ret.Count();
            });
        
        // Act 
        int actual = await this._productsService
            .GetSpecifiedProductsCountAsync(Guid.NewGuid(), approved, searchQuery);
        
        // Assert
        int expected;
        if (approved is false)
        {
            expected = !string.IsNullOrWhiteSpace(searchQuery)
                ? products.Count(p => p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved &&
                                      (p.Name.ToLower().Contains(searchQuery.ToLower()) ||
                                       p.Category.Name.ToLower().Contains(searchQuery.ToLower())))
                : products.Count(p => p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved);
        }
        else if (approved is true)
        {
            expected = !string.IsNullOrWhiteSpace(searchQuery)
                ? products.Count(p => p.ApprovalDecision.ApprovalStatus == ApprovalStatus.Approved &&
                                      (p.Name.ToLower().Contains(searchQuery.ToLower()) ||
                                       p.Category.Name.ToLower().Contains(searchQuery.ToLower())))
                : products.Count(p => p.ApprovalDecision.ApprovalStatus == ApprovalStatus.Approved);
        }
        else
        {
            expected = !string.IsNullOrWhiteSpace(searchQuery)
                ? products.Count(p => p.Name.ToLower().Contains(searchQuery.ToLower()) ||
                                      p.Category.Name.ToLower().Contains(searchQuery.ToLower()))
                : products.Count();
        }

        Assert.That(actual, Is.EqualTo(expected));
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

    /// <summary>
    /// Orchestrates Test Collection
    /// </summary>
    /// <param name="productId">If passed first item in the collection will take this identifier.</param>
    /// <param name="categoryIdWithCategoryName">
    /// Value Tuple holding data for category id and category name.
    /// The values will be set on the first three elements in the collection.
    /// </param>
    /// <param name="withSomeDisapproved">If set to true the first element will be with disapproved status.</param>
    /// <returns>Test collection of Products</returns>
    private static IEnumerable<Product> SetupProductsCollection(
        Guid? productId = null,
        ValueTuple<Guid, string>? categoryIdWithCategoryName = null,
        bool withSomeDisapproved = false)
    {
        productId ??= Guid.NewGuid();
        categoryIdWithCategoryName ??= new ValueTuple<Guid, string>(Guid.NewGuid(), "Random Category");
        
        return new List<Product>()
        {
            new Product()
            {
                Id = productId.Value, Name = "Big TV", CategoryId = categoryIdWithCategoryName.Value.Item1,
                ApprovalDecision = new ApprovalDecision { ApprovalStatus = withSomeDisapproved ? ApprovalStatus.Disapproved : ApprovalStatus.Approved },
                Category = new Category() { Name = categoryIdWithCategoryName.Value.Item2 }
            },
            new Product()
            {
                Id = Guid.NewGuid(), Name = "small TV", CategoryId = categoryIdWithCategoryName.Value.Item1,
                ApprovalDecision = new ApprovalDecision { ApprovalStatus = ApprovalStatus.Approved },
                Category = new Category() { Name = categoryIdWithCategoryName.Value.Item2 }
            },
            new Product()
            {
                Id = Guid.NewGuid(), Name = "Basketball", CategoryId = categoryIdWithCategoryName.Value.Item1,
                ApprovalDecision = new ApprovalDecision { ApprovalStatus = ApprovalStatus.Approved },
                Category = new Category() { Name = categoryIdWithCategoryName.Value.Item2 }
            },
            new Product()
            {
                Id = Guid.NewGuid(), Name = "Football", CategoryId = Guid.NewGuid(),
                ApprovalDecision = new ApprovalDecision { ApprovalStatus = ApprovalStatus.Approved },
                Category = new Category() { Name = "some name" }
            },
            new Product()
            {
                Id = Guid.NewGuid(), Name = "Volleyball", CategoryId = Guid.NewGuid(),
                ApprovalDecision = new ApprovalDecision { ApprovalStatus = ApprovalStatus.Approved },
                Category = new Category() { Name = "TV Category" }
            },
        };
    }
}