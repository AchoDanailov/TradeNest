using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;

namespace TradeNest.Data.IntegrationTests.Repositories;

public class CategoriesRepositoryTests : RepositoryTestsBase
{
    private CategoriesRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new CategoriesRepository(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
    }

    [Test]
    public async Task AddAsync_ShouldAddCategoryToDatabase()
    {
        // Arrange
        var category = new Category { Name = "New Category" };

        // Act
        var result = await _repository.AddAsync(category);

        // Assert
        Assert.True(result);
        var dbCategory = await DbContext.Categories.FirstOrDefaultAsync(c => c.Name == "New Category");
        Assert.NotNull(dbCategory);
    }

    [Test]
    public async Task DeleteCategoryAsync_ShouldRemoveCategory()
    {
        // Arrange
        var category = new Category { Name = "To Delete" };
        await SeedAsync(category);

        // Act
        var result = await _repository.DeleteCategoryAsync(category);

        // Assert
        Assert.True(result);
        var dbCategory = await DbContext.Categories.FindAsync(category.Id);
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
        var result = await _repository.GetAllAsync(options => options.SetFilter(c => c.Name == "Books"));

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("Books"));
    }
}