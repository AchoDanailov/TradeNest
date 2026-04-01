using Microsoft.AspNetCore.Identity;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class UsersRepository : BaseReadRepository<ApplicationUser>, IUsersRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UsersRepository(TradeNestDbContext dbContext, UserManager<ApplicationUser> userManager) 
        : base(dbContext)
    {
        this._userManager = userManager;
    }

    public async Task<bool> AddAsync(ApplicationUser user, string password)
    {
        IdentityResult result = await this._userManager.CreateAsync(user, password);
        return result.Succeeded;
    }
}