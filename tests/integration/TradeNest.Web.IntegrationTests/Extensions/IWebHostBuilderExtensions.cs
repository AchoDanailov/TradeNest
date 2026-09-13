using Microsoft.AspNetCore.TestHost;
using TradeNest.Web.IntegrationTests.TestsServices;

namespace TradeNest.Web.IntegrationTests.Extensions;

/// <summary>
/// A class containing extension methods for the <see cref="IWebHostBuilder"/> interface.
/// </summary>
public static class IWebHostBuilderExtensions
{
    /// <summary>
    /// Configures an HTTP GET resource for obtaining valid antiforgery tokens.
    /// </summary>
    /// <param name="builder">The <see cref="IWebHostBuilder"/> to configure.</param>
    /// <returns>
    /// The <see cref="IWebHostBuilder"/> specified by <paramref name="builder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    public static IWebHostBuilder ConfigureAntiforgeryTokenResource(this IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        
        builder.ConfigureTestServices(services =>
        {
            services.AddControllersWithViews()
                .AddApplicationPart(typeof(AntiforgeryTokenController).Assembly)
                .AddControllersAsServices();
        });

        return builder;
    }
}