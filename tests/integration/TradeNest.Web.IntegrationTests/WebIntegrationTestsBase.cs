using System.Net.Http.Headers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

using TradeNest.Web.IntegrationTests.Common;
using TradeNest.Web.IntegrationTests.Infrastructure;
using TradeNest.Web.IntegrationTests.TestsServices;

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
    
    /// <summary>
    /// Creates a new <see cref="HttpClient"/> pointing towards a new <see cref="WebApplicationFactory{TEntryPoint}"/>
    /// configured with the <see cref="TestAuthHandler"/>.
    /// <see cref="WebApplicationFactoryClientOptions"/> options set - AllowAutoRedirect to false and BaseAddress to "https://localhost"
    /// </summary>
    /// <param name="clientOptions">
    /// Optional <see cref="WebApplicationFactoryClientOptions"/> allowing configuration of the <see cref="HttpClient"/>
    /// </param>
    /// <returns>The configured <see cref="HttpClient"/></returns>
    protected HttpClient SetupAuthenticatedHttpClient(
        WebApplicationFactoryClientOptions? clientOptions = null)
    {
        HttpClient client = this.Factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: TestsConstants.Auth.Scheme)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestsConstants.Auth.Scheme, options => { });
                });
            })
            .CreateClient(clientOptions ?? new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });    
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: TestsConstants.Auth.Scheme);

        return client;
    }
}