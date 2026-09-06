using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

using TradeNest.Data;
using TradeNest.Web.IntegrationTests.Extensions;
using TradeNest.Web.IntegrationTests.Models;

namespace TradeNest.Web.IntegrationTests;

internal class TradeNestTestsWebApplicationFactory<TProgram> 
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _databaseConnectionString;

    internal TradeNestTestsWebApplicationFactory(string databaseConnectionString)
        : base()
    {
        this._databaseConnectionString = databaseConnectionString;
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
    
    // this builder is the same builder of the SUT's WebApplication builder before builder.Build() is called but after the configurations happen in Program.cs
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
        });

        builder.ConfigureAntiforgeryTokenResource();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            using (IServiceScope scope = this.Services.CreateScope())
            {
                TradeNestDbContext dbContext = scope.ServiceProvider.GetRequiredService<TradeNestDbContext>();
                dbContext.Database.EnsureDeleted();
            }
        }
        
        base.Dispose(disposing);
    }
}