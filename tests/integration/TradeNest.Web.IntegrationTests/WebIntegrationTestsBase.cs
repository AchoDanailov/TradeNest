using NUnit.Framework;
using TradeNest.Web.IntegrationTests.Infrastructure;

namespace TradeNest.Web.IntegrationTests;

[TestFixture]
public class WebIntegrationTestsBase
{
    protected TradeNestTestsWebApplicationFactory<Program> Factory { get; private set; } 

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        string connectionString = MsSqlDbContainer.Instance().GetConnectionString();
        this.Factory = new TradeNestTestsWebApplicationFactory<Program>(connectionString);
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // NOTE: The TradeNestWebApplicationFactory's Dispose method takes care of dropping the db.
        this.Factory.Dispose();
    }
}