using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

using TradeNest.Data.Models;
using TradeNest.Data.Repository;

namespace TradeNest.Data.Tests.RepositoriesTests;

public class CategoriesRepositoryTests : BaseRepositoriesTest
{
    private CategoriesRepository _categoriesRepository;

    [SetUp]
    public async Task SetUp()
    {
        this._categoriesRepository = new CategoriesRepository(this.DbContext);
        await base.SetUp();
    }

    [TearDown]
    public async Task TearDown()
    {
        this._categoriesRepository.Dispose();
        await base.TearDown();
    }

    [Test]
    public async Task AddAsync_WhenExists_ReturnsFalse()
    {
        // Arrange
        Category category = new Category() { Id = Guid.NewGuid() };
        await this.SeedAsync(category);
        this.DbContext.Entry(category).State = EntityState.Detached;

        // Act
        bool res = await this._categoriesRepository.AddAsync(category);

        // Assert
        Assert.That(res, Is.False);
    }

    [Test]
    public async Task AddAsync_WorksCorrectly()
    {
        // Arrange
        Category category = new Category() { Id = Guid.NewGuid() };

        // Act
        bool isAdded = await this._categoriesRepository.AddAsync(category);
        this.DbContext.Entry(category).State = EntityState.Detached;
        Category? fetched = await this.DbContext.Categories.FindAsync(category.Id);

        // Assert
        Assert.That(isAdded, Is.True);
        Assert.NotNull(fetched);       
        Assert.That(fetched.Id, Is.EqualTo(category.Id));
    }

    [Test]
    public async Task DeleteCategoryAsync_WhenDoesNotExist_ReturnsFalse()
    {
        // Arrange && Act
        bool res = await this._categoriesRepository
            .DeleteCategoryAsync(It.IsAny<Category>());

        // Assert
        Assert.That(res, Is.False);
    }

    [Test]
    public async Task DeleteCategoryAsync_WorksCorrectly()
    {
        // Arrange
        Category category = new Category() { Id = Guid.NewGuid() };
        await this.SeedAsync(category);
        this.DbContext.Entry(category).State = EntityState.Detached;

        // Act
        bool isDeleted = await this._categoriesRepository.DeleteCategoryAsync(category);
        this.DbContext.Entry(category).State = EntityState.Detached;
        Category? fetched = await this.DbContext.Categories.FindAsync(category.Id);

        // Assert
        Assert.That(isDeleted, Is.True);
        Assert.IsNull(fetched);
    }
}
