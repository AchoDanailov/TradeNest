using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Models;
using TradeNest.Data.Repository;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Tests.Common;

public class TestReadRepository<T> : BaseReadRepository<T>
    where T : class, new()
{
    public TestReadRepository(TradeNestDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<T> PublicToQueryable(Action<QueryOptions<T>> queryOptionsBuilder, IQueryable<T>? queryable = null)
    {
        return this.ToQueryable(queryOptionsBuilder, queryable);
    }
}

[TestFixture]
public class QueryOptionsTests
{
    private TradeNestDbContext _dbContext;
    private TestReadRepository<Category> _repository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TradeNestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new TradeNestDbContext(options);
        _repository = new TestReadRepository<Category>(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
        _dbContext.Dispose();
    }

    [Test]
    public void ToQueryable_ShouldApplyFilter_WhenFilterIsProvided()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Electronics" },
            new Category { Id = Guid.NewGuid(), Name = "Books" }
        }.AsQueryable();

        // Act
        var result = _repository.PublicToQueryable(options => options.AddFilter(c => c.Name == "Electronics"), categories);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("Electronics"));
    }

    [Test]
    public void ToQueryable_ShouldApplyOrdering_WhenOrderIsProvided()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Electronics" },
            new Category { Id = Guid.NewGuid(), Name = "Books" }
        }.AsQueryable();

        // Act
        var result = _repository.PublicToQueryable(options => options.AddOrderAsc(c => c.Name), categories);

        // Assert
        var list = result.ToList();
        Assert.That(list[0].Name, Is.EqualTo("Books"));
        Assert.That(list[1].Name, Is.EqualTo("Electronics"));
    }

    [Test]
    public void ToQueryable_ShouldApplyPagination_WhenPageAndLimitAreProvided()
    {
        // Arrange
        var categories = new List<Category>();
        for (int i = 1; i <= 10; i++)
        {
            categories.Add(new Category { Id = Guid.NewGuid(), Name = $"Category {i:D2}" });
        }
        var queryable = categories.AsQueryable();

        // Act
        var result = _repository.PublicToQueryable(options => options.WithPagination(2, 3), queryable);

        // Assert
        var list = result.ToList();
        Assert.That(list.Count, Is.EqualTo(3));
        // Page 2 with limit 3 skips 3, so we get 4th, 5th, 6th
        Assert.That(list[0].Name, Is.EqualTo("Category 04"));
    }

    [Test]
    public void ToQueryable_ShouldApplyAsNoTracking_WhenAsReadOnlyIsCalled()
    {
        // Arrange
        var categories = _dbContext.Categories;

        // Act
        var result = _repository.PublicToQueryable(options => options.AsReadOnly(), categories);

        // Assert
        // In EF Core, we can check for NoTracking by looking at the Queryable string or internal state,
        // but a simpler way is to verify it doesn't throw and behaves like a NoTracking query.
        Assert.That(result.Expression.ToString(), Does.Contain(".AsNoTracking()"));
    }
}