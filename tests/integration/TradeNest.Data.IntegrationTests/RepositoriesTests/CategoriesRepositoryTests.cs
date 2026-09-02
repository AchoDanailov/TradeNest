using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using static TradeNest.Tests.Common.RandomStringGenerator;

namespace TradeNest.Data.IntegrationTests.RepositoriesTests;

public class CategoriesRepositoryTests : IntegrationTestsBase
{
    private CategoriesRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        this._repository = new CategoriesRepository(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        this._repository.Dispose();
    }

    [Test]
    public async Task AddAsync_ShouldAddCategoryToDatabase()
    {
        // Arrange
        Category category = new Category { Name = RandomString(3, 15) };

        // Act
        bool result = await _repository.AddAsync(category);

        // Assert
        Assert.True(result);
        Category? dbCategory = await DbContext.Categories
            .FirstOrDefaultAsync(c => c.Name == category.Name);
        Assert.NotNull(dbCategory);
    }

    [Test]
    public async Task DeleteCategoryAsync_ShouldRemoveCategory()
    {
        // Arrange
        Category category = new Category { Id = Guid.NewGuid(), Name = RandomString(3, 15) };
        await SeedAsync(category);

        // Act
        bool result = await _repository.DeleteCategoryAsync(category);

        // Assert
        Assert.True(result);
        Category? dbCategory = await DbContext.Categories.FindAsync(category.Id);
        Assert.Null(dbCategory);
    }

    [Test]
    public async Task GetAllAsync_WithFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        await SeedAsync(
            new Category { Name = "Electronics" },
            new Category { Name = "Books" }
        );

        // Act
        IEnumerable<Category> result = (await _repository
                .GetAllAsync(options => options.SetFilter(c => c.Name == "Books")))
            .ToArray();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("Books"));
    }
}