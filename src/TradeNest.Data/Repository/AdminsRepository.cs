using Microsoft.AspNetCore.Identity;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class AdminsRepository : BaseReadRepository<Admin>, IAdminsRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AdminsRepository(TradeNestDbContext dbContext, UserManager<ApplicationUser> userManager) 
        : base(dbContext)
    {
        this._userManager = userManager;
    }

    public async Task<bool> AddAsync(Admin admin)
    {
        await this.DbContext.Admins.AddAsync(admin);
        IdentityResult addToRoleResult = await this._userManager
            .AddToRoleAsync(admin.User, "Admin"); 
        
        return addToRoleResult.Succeeded;
    }

    public async Task<bool> AddRangeAsync(IEnumerable<Admin> admins)
    {
        bool allAddingToRolesSuccess = true;
        
        foreach (Admin admin in admins)
            allAddingToRolesSuccess &= await this.AddAsync(admin);
        
        return allAddingToRolesSuccess;
    }
}