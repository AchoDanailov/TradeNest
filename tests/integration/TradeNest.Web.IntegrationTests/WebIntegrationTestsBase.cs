using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

using TradeNest.Web.IntegrationTests.Models;
using TradeNest.Web.IntegrationTests.Infrastructure;

namespace TradeNest.Web.IntegrationTests;

[TestFixture]
public class WebIntegrationTestsBase
{
    private TradeNestTestsWebApplicationFactory<Program> _factory;

    protected HttpClient HttpClient { get; private set; }
    protected AntiforgeryTokens AntiforgeryTokens
        => this.GetAntiforgeryTokensAsync().GetAwaiter().GetResult();

    [SetUp]
    public void SetUp()
    {
        // NOTE: If web integration tests get too much, might have to consider using one db state per TestFixture.
        string dbConnectionString = MsSqlDbContainer.Instance().GetConnectionString();
        this._factory = new TradeNestTestsWebApplicationFactory<Program>(dbConnectionString);
        
        this.HttpClient = this._factory
            .CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false });
    }

    [TearDown]
    public void TearDown()
    {
        // NOTE: The TradeNestWebApplicationFactory's Dispose method takes care of dropping the db.
        this._factory.Dispose();
    }

    private async Task<AntiforgeryTokens> GetAntiforgeryTokensAsync() 
        => await this._factory.GetAntiforgeryTokensAsync(this.HttpClient);
}