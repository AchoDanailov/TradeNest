using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TradeNest.Data;
using TradeNest.Data.Models;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Web.Infrastructure.Extensions;
using TradeNest.Web.Infrastructure.Filters;
using TradeNest.Web.Mappers.Interfaces;

namespace TradeNest.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        string connectionString = builder.Configuration["TradeNest:ConnectionString"] 
                                  ?? builder.Configuration.GetConnectionString("DefaultConnection") 
                                  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<TradeNestDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services
            .AddDefaultIdentity<ApplicationUser>(options => 
                IdentityOptionsConfiguration(options, builder.Configuration))
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<TradeNestDbContext>();

        builder.Services.ConfigureApplicationCookie(options =>
            ApplicationCookieConfiguration(options, builder.Configuration));

        builder.Services.AddScoped<WebApiExceptionFilter>();

        builder.Services.AddControllersWithViews();

        // Required for static assets to work properly when app is not launched in development environment and has yet not been published.
        // Should be removed once the app is published.
        builder.WebHost.UseStaticWebAssets();

        builder.Services.RegisterRepositories(typeof(IProductsRepository).Assembly);
        
        builder.Services.RegisterMappings(typeof(IProductsMapper).Assembly, 
            typeof(IProductPresentationModelsMapper).Assembly);
        
        builder.Services.RegisterUserServices(typeof(IProductsService).Assembly);

        WebApplication app = builder.Build();

        app.UseExceptionHandler("/Error");

        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Error/StatusCode/{0}");

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

    private static void ApplicationCookieConfiguration(CookieAuthenticationOptions options,
        ConfigurationManager configuration)
    {
        IConfigurationSection section = configuration.GetSection("CookieAuthOptions");
        
        options.Cookie.HttpOnly = section.GetValue<bool>("Cookie:HttpOnly");
        options.Cookie.SameSite = (SameSiteMode)section.GetValue<int>("Cookie:SameSite");
        options.Cookie.SecurePolicy = (CookieSecurePolicy)section.GetValue<int>("Cookie:SecurePolicy");
        
        options.ExpireTimeSpan = TimeSpan.FromMinutes(section.GetValue<int>("ExpireTimeSpan"));
        options.SlidingExpiration = section.GetValue<bool>("SlidingExpiration");
        options.AccessDeniedPath = section.GetValue<string>("AccessDeniedPath");
    }
}