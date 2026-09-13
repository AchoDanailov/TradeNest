using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

using TradeNest.Data;
using TradeNest.Web.IntegrationTests.Extensions;
using TradeNest.Web.IntegrationTests.Models;
using TradeNest.Web.IntegrationTests.TestsServices;

namespace TradeNest.Web.IntegrationTests;

public class TradeNestTestsWebApplicationFactory<TProgram> 
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _databaseConnectionString;

    public TradeNestTestsWebApplicationFactory(string databaseConnectionString)
        : base()
    {
        this._databaseConnectionString = databaseConnectionString;
    }
    
    /// <summary>
    /// Method used to configure an InMemory Web Host used to test against.
    /// The web test host has all the SUT's services already registered and configured.
    /// </summary>
    /// <param name="builder">
    /// <see cref="IWebHostBuilder"/> instance used to add, replace, setup, and configure services for testing purposes.
    /// </param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // docs: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0&pivots=nunit
            ServiceDescriptor? serviceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<TradeNestDbContext>));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }
            
            services.AddDbContext<TradeNestDbContext>(options =>
                options.UseSqlServer(this._databaseConnectionString));

            using ServiceProvider serviceProvider = services
                .BuildServiceProvider();
            using TradeNestDbContext dbContext = serviceProvider
                .GetRequiredService<TradeNestDbContext>();
            dbContext.Database.EnsureCreated();
        });

        builder.ConfigureAntiforgeryTokenResource();
        builder.UseEnvironment("Development");
    }
    
    /// <summary>
    /// Gets a set of valid antiforgery tokens for the application as an asynchronous operation.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get a set of valid
    /// antiforgery (CSRF/XSRF) tokens to use for HTTP POST requests to the test server.
    /// </returns>
    internal async Task<AntiforgeryTokens> GetAntiforgeryTokensAsync(HttpClient httpClient)
    {
        AntiforgeryTokens? tokens = await httpClient
            .GetFromJsonAsync<AntiforgeryTokens>(AntiforgeryTokenController.GetTokensUri); 
        return tokens!;
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            using IServiceScope scope = this.Services.CreateScope();
            using TradeNestDbContext dbContext = scope.ServiceProvider
                .GetRequiredService<TradeNestDbContext>();
            dbContext.Database.EnsureDeleted();
        }
        
        base.Dispose(disposing);
    }
}