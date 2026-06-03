using Moq;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core;
using TradeNest.Services.Models.Category;

namespace TradeNest.Services.Tests.Categories;

[TestFixture]
public class CategoriesServiceTests
{
    private Mock<ICategoriesRepository> _categoriesRepositoryMock;
    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<IAdminsRepository> _adminsRepositoryMock;
    private CategoriesService _categoriesService;

    [SetUp]
    public void SetUp()
    {
        _categoriesRepositoryMock = new Mock<ICategoriesRepository>();
        _productsRepositoryMock = new Mock<IProductsRepository>();
        _adminsRepositoryMock = new Mock<IAdminsRepository>();

        _categoriesService = new CategoriesService(
            _categoriesRepositoryMock.Object,
            _productsRepositoryMock.Object,
            _adminsRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllCategoriesAsync_ShouldReturnCategories_OrderedByName()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Electronics" },
            new Category { Id = Guid.NewGuid(), Name = "Books" }
        };

        _categoriesRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<Action<IQueryOptions<Category>>>()))
            .ReturnsAsync(categories.OrderBy(c => c.Name));

        // Act
        var result = await _categoriesService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList[0].CategoryName, Is.EqualTo("Books"));
        Assert.That(resultList[1].CategoryName, Is.EqualTo("Electronics"));
    }

    [Test]
    public async Task GetAllCategoriesWithBestSellerImageAsync_ShouldReturnCategoriesWithImages()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categories = new List<Category>
        {
            new Category { Id = categoryId, Name = "Electronics" }
        };

        var bestSellerImages = new Dictionary<Guid, string?>
        {
            { categoryId, "image-url" }
        };

        _categoriesRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<Action<IQueryOptions<Category>>>()))
            .ReturnsAsync(categories);

        _productsRepositoryMock
            .Setup(r => r.GetAllCategoriesBestSellersFrontImagesAsync(It.IsAny<bool>()))
            .ReturnsAsync(bestSellerImages);

        // Act
        var result = await _categoriesService.GetAllCategoriesWithBestSellerImageAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.That(resultList[0].BestSellerImageUrl, Is.EqualTo("image-url"));
    }

    [Test]
    public void CreateCategoryAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => 
            await _categoriesService.CreateCategoryAsync(Guid.Empty, "New Category"));
    }

    [Test]
    public void CreateCategoryAsync_ShouldThrowArgumentException_WhenCategoryNameIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => 
            await _categoriesService.CreateCategoryAsync(Guid.NewGuid(), ""));
    }

    [Test]
    public void CreateCategoryAsync_ShouldThrowUnauthorizedOperationException_WhenUserIsNotAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedOperationException>(async () => 
            await _categoriesService.CreateCategoryAsync(userId, "New Category"));
    }

    [Test]
    public async Task CreateCategoryAsync_ShouldReturnCategoryId_WhenSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryName = "New Category";
        var categoryId = Guid.NewGuid();

        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(userId))
            .ReturnsAsync(true);

        _categoriesRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(true);

        _categoriesRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<Action<IQueryOptions<Category>>>()))
            .ReturnsAsync(new List<Category> { new Category { Id = categoryId, Name = categoryName } });

        // Act
        var result = await _categoriesService.CreateCategoryAsync(userId, categoryName);

        // Assert
        Assert.That(result, Is.EqualTo(categoryId));
        _categoriesRepositoryMock.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == categoryName)), Times.Once);
    }

    [Test]
    public void DeleteCategoryByIdAsync_ShouldThrowArgumentException_WhenIdsAreEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => 
            await _categoriesService.DeleteCategoryByIdAsync(Guid.Empty, Guid.NewGuid()));
        Assert.ThrowsAsync<ArgumentException>(async () => 
            await _categoriesService.DeleteCategoryByIdAsync(Guid.NewGuid(), Guid.Empty));
    }

    [Test]
    public void DeleteCategoryByIdAsync_ShouldThrowResourceNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoriesRepositoryMock
            .Setup(r => r.FindByIdAsync(categoryId))
            .ReturnsAsync((Category)null);

        // Act & Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(async () => 
            await _categoriesService.DeleteCategoryByIdAsync(Guid.NewGuid(), categoryId));
    }

    [Test]
    public async Task DeleteCategoryByIdAsync_ShouldReturnFailure_WhenDeletingDefaultCategory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = ApplicationConstants.DefaultProductsCategory };

        _categoriesRepositoryMock
            .Setup(r => r.FindByIdAsync(categoryId))
            .ReturnsAsync(category);

        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _categoriesService.DeleteCategoryByIdAsync(userId, categoryId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.That(result.FailureReason, Is.EqualTo(ExpectedFailureReason.RemovingDefaultCategory));
    }

    [Test]
    public async Task DeleteCategoryByIdAsync_ShouldDeleteDirectly_WhenCategoryHasNoProducts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Empty Category" };

        _categoriesRepositoryMock
            .Setup(r => r.FindByIdAsync(categoryId))
            .ReturnsAsync(category);

        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(userId))
            .ReturnsAsync(true);

        _productsRepositoryMock
            .Setup(r => r.ExistsIncludingArchivedAndNotApprovedAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .ReturnsAsync(false);

        _categoriesRepositoryMock
            .Setup(r => r.DeleteCategoryAsync(category))
            .ReturnsAsync(true);

        // Act
        var result = await _categoriesService.DeleteCategoryByIdAsync(userId, categoryId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.WereProductsMoved);
        _categoriesRepositoryMock.Verify(r => r.DeleteCategoryAsync(category), Times.Once);
    }

    [Test]
    public async Task DeleteCategoryByIdAsync_ShouldMoveProductsAndDelete_WhenCategoryHasProducts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Populated Category" };
        var defaultCategoryId = Guid.NewGuid();
        var defaultCategory = new Category { Id = defaultCategoryId, Name = ApplicationConstants.DefaultProductsCategory };

        _categoriesRepositoryMock
            .Setup(r => r.FindByIdAsync(categoryId))
            .ReturnsAsync(category);

        _adminsRepositoryMock
            .Setup(r => r.IsUserAdminByUserIdAsync(userId))
            .ReturnsAsync(true);

        _productsRepositoryMock
            .Setup(r => r.ExistsIncludingArchivedAndNotApprovedAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .ReturnsAsync(true);

        _categoriesRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<Action<IQueryOptions<Category>>>()))
            .ReturnsAsync(new List<Category> { defaultCategory });

        _productsRepositoryMock
            .Setup(r => r.ExecuteUpdateProductsRangeCategoriesIdsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), defaultCategoryId))
            .ReturnsAsync(true);

        _categoriesRepositoryMock
            .Setup(r => r.DeleteCategoryAsync(category))
            .ReturnsAsync(true);

        // Act
        var result = await _categoriesService.DeleteCategoryByIdAsync(userId, categoryId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.WereProductsMoved);
        _productsRepositoryMock.Verify(r => r.ExecuteUpdateProductsRangeCategoriesIdsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), defaultCategoryId), Times.Once);
        _categoriesRepositoryMock.Verify(r => r.DeleteCategoryAsync(category), Times.Once);
    }
}