using NUnit.Framework;
using Testcontainers.MsSql;

using TradeNest.Data.IntegrationTests.Infrastructure;

namespace TradeNest.Data.IntegrationTests;

/// <summary>
/// This class is a SetUpFixture. It executes ONCE prior to all TestFixtures in the namespace.
/// https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html
/// </summary>
[SetUpFixture]
public class SetUpFixture
{
    private MsSqlContainer _dbContainer;
    
    [OneTimeSetUp]
    public async Task SetUpFixtureAsync()
    {
        this._dbContainer = MsSqlDbContainer.Instance();
        await this._dbContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDownFixtureAsync()
    {
        await this._dbContainer.StopAsync();
        await this._dbContainer.DisposeAsync();
    }
}