using Microsoft.AspNetCore.Identity;
using TradeNest.Data.Models;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Data.Seeding.Interfaces;

namespace TradeNest.Data.Seeding;

public class RolesSeeder : IRolesSeeder
{
    private static readonly IEnumerable<string> ApplicationRolesNames = new string[]
    {
        "Admin",
    };
    
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RolesSeeder(RoleManager<ApplicationRole> roleManager)
    {
        this._roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        foreach (string roleName in ApplicationRolesNames)
        {
            if (await this._roleManager.RoleExistsAsync(roleName))
                continue;

            ApplicationRole newRole = new ApplicationRole(roleName);
            IdentityResult res = await this._roleManager.CreateAsync(newRole);
            if (!res.Succeeded)
            {
                throw new InvalidOperationException(string.Format(SeedingError,
                    this.GetType().Name));
            }
        }   
    }
}