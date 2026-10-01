using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace TradeNest.Data.Tests.RepositoriesTests;

[TestFixture]
public abstract class BaseRepositoriesTest
{
    protected TradeNestDbContext DbContext { get; private set; }
    
    [SetUp]
    public async Task SetUp()
    {
        DbContextOptions<TradeNestDbContext> options = new DbContextOptionsBuilder<TradeNestDbContext>()
            .UseInMemoryDatabase(databaseName: "TestingDatabase", inMemoryOptions =>
                inMemoryOptions.EnableNullChecks(nullChecksEnabled: false))
            .Options;

        this.DbContext = new TradeNestDbContext(options);
        await this.DbContext.Database.EnsureCreatedAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        try
        {
            await this.DbContext.Database.EnsureDeletedAsync();
            await this.DbContext.DisposeAsync();
        }
        catch (ObjectDisposedException)
        {
            // Already disposed by repository
        }
    }
    
    protected async Task SeedAsync<T>(params T[] entities) 
        where T : class
    {
        await DbContext.Set<T>().AddRangeAsync(entities);
        await DbContext.SaveChangesAsync();
    }
}