using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

using TradeNest.Data.IntegrationTests.Infrastructure;

namespace TradeNest.Data.IntegrationTests;

[TestFixture]
public abstract class IntegrationTestsBase
{
    protected TradeNestDbContext DbContext { get; private set; }

    [SetUp]
    public async Task BaseSetUpAsync()
    {
        DbContextOptions<TradeNestDbContext> options = new DbContextOptionsBuilder<TradeNestDbContext>()
            .UseSqlServer(MsSqlDbContainer.Instance().GetConnectionString())
            .Options;
        
        this.DbContext = new TradeNestDbContext(options);
        await this.DbContext.Database.EnsureCreatedAsync();
    }

    [TearDown]
    public async Task BaseTearDownAsync()
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