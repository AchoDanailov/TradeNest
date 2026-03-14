using Microsoft.AspNetCore.Identity;
using TradeNest.Data;
using TradeNest.Data.Models;
using TradeNest.Services.Core;
using TradeNest.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Repository;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string? connectionString = builder.Configuration["TradeNest:ConnectionString"] 
                                   ?? builder.Configuration.GetConnectionString("DefaultConnection") 
                                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<TradeNestDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
                IdentityOptionsConfiguration(options, builder.Configuration))
            
            .AddEntityFrameworkStores<TradeNestDbContext>();

        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IRepository, Repository>();

        builder.Services.AddScoped<IProductsService, ProductsService>();
        builder.Services.AddScoped<ICategoriesService, CategoriesService>();
        builder.Services.AddScoped<IOrdersService, OrdersService>();
        builder.Services.AddScoped<ICartsService, CartsService>();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Error/StatusCode", "?statusCode={0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapRazorPages();

        app.Run();
    }

    private static void IdentityOptionsConfiguration(IdentityOptions options,
        ConfigurationManager configuration)
    {
        options.SignIn.RequireConfirmedAccount = configuration
            .GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedAccount");
        options.SignIn.RequireConfirmedEmail = configuration
            .GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedEmail");
        options.SignIn.RequireConfirmedPhoneNumber = configuration
            .GetValue<bool>("IdentityOptions:SignIn:RequireConfirmedPhoneNumber");

        options.User.RequireUniqueEmail = configuration
            .GetValue<bool>("IdentityOptions:User:RequireUniqueEmail");

        int defaultLockoutMinutes = configuration
            .GetValue<int>("IdentityOptions:Lockout:DefaultLockoutTimeSpan");
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(defaultLockoutMinutes);
        options.Lockout.MaxFailedAccessAttempts = configuration
            .GetValue<int>("IdentityOptions:Lockout:MaxFailedAccessAttempts");

        options.Password.RequireDigit = configuration
            .GetValue<bool>("IdentityOptions:Password:RequireDigit");
        options.Password.RequiredLength = configuration
            .GetValue<int>("IdentityOptions:Password:RequiredLength");
        options.Password.RequireUppercase = configuration
            .GetValue<bool>("IdentityOptions:Password:RequireUppercase");
        options.Password.RequireLowercase = configuration
            .GetValue<bool>("IdentityOptions:Password:RequireLowercase");
        options.Password.RequireNonAlphanumeric = configuration
            .GetValue<bool>("IdentityOptions:Password:RequireNonAlphanumeric");
    }
}