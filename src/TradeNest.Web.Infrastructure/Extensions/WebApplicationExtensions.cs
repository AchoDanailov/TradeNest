using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using TradeNest.Data.Seeding.Interfaces;

namespace TradeNest.Web.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseSeeding(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        IEnumerable<IEntitySeeder> entitySeeders = new List<IEntitySeeder>()
        {
            scope.ServiceProvider.GetRequiredService<IUsersSeeder>(),
            scope.ServiceProvider.GetRequiredService<IAdminsSeeder>(),
            scope.ServiceProvider.GetRequiredService<ICategoriesSeeder>(),
            scope.ServiceProvider.GetRequiredService<IProductsSeeder>()
        };

        foreach (IEntitySeeder entitySeeder in entitySeeders)
        {
            entitySeeder
                .SeedEntityDataAsync()
                .GetAwaiter()
                .GetResult();
        }

        return app;
    }
}