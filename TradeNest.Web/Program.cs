using Microsoft.AspNetCore.Identity;
using TradeNest.Data;
using TradeNest.Data.Models;
using TradeNest.Services.Core;
using TradeNest.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        
        builder.Services.AddDefaultIdentity<ApplicationUser>(options => IdentityOptionsConfiguration(options))
            .AddEntityFrameworkStores<TradeNestDbContext>();

        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IProductsService, ProductsService>();
        builder.Services.AddScoped<ICategoriesService, CategoriesService>();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

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

    private static void IdentityOptionsConfiguration(IdentityOptions options)
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;

        options.User.RequireUniqueEmail = true;
                
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
        options.Lockout.MaxFailedAccessAttempts = Int32.MaxValue;

        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 2;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
    }
}