using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace TradeNest.Data.IntegrationTests;

public abstract class RepositoryTestsBase
{
    protected TradeNestDbContext DbContext { get; private set; } = null!;

    [SetUp]
    public void BaseSetUp()
    {
        var options = new DbContextOptionsBuilder<TradeNestDbContext>()
            .UseInMemoryDatabase(databaseName: "TradeNestTestDb")
            .Options;

        DbContext = new TradeNestDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    [TearDown]
    public void BaseTearDown()
    {
        try
        {
            DbContext.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Already disposed by repository
        }
    }

    protected async Task SeedAsync<T>(params T[] entities) where T : class
    {
        await DbContext.Set<T>().AddRangeAsync(entities);
        await DbContext.SaveChangesAsync();
    }
}