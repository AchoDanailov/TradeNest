using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;

namespace TradeNest.Data.Repository;

public class UsersRepository : BaseReadRepository<ApplicationUser>, IUsersRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UsersRepository(TradeNestDbContext dbContext,
        UserManager<ApplicationUser> userManager) 
        : base(dbContext)
    {
        this._userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUser>>
        GetAllUsersWithTheirRolesAsync(bool asReadOnly = false)
    {
        IQueryable<ApplicationUser> queryable = this.DbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role);
        if (asReadOnly)
            queryable = queryable.AsNoTracking();

        return await queryable.ToArrayAsync();
    }

    public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync(bool asReadOnly = false)
    {
        IQueryable<ApplicationRole> queryable = this.DbContext.Roles;
        if (asReadOnly)
            queryable = queryable.AsNoTracking();

        return await queryable.ToArrayAsync();
    }

    public async Task<bool> ExistsByIdWithForgottenIncludedAsync(Guid userId, bool asReadOnly = false)
    {
        IQueryable<ApplicationUser> queryable = this.DbContext.Users.IgnoreQueryFilters();
        if (asReadOnly)
            queryable = queryable.AsNoTracking();

        return await queryable.AnyAsync(u => u.Id == userId);
    }

    public async Task<ApplicationUser?> FindByIdWithForgottenIncludedAsync(Guid userId)
    {
        return await this.DbContext.Users
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(u => u.Id == userId);
    }
    
    public async Task<bool> AddAsync(ApplicationUser user, string password)
    {
        IdentityResult result = await this._userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task DeleteAsync(ApplicationUser user)
    {
        await using IDbContextTransaction transaction
            = await this.DbContext.Database.BeginTransactionAsync();

        try
        {
            this.DbContext.Attach(user);

            await this._userManager.SetUserNameAsync(user, null);
            await this._userManager.SetEmailAsync(user, null);
            await this._userManager.UpdateNormalizedUserNameAsync(user);
            await this._userManager.UpdateNormalizedEmailAsync(user);
            await this._userManager.RemovePasswordAsync(user);

            IEnumerable<string> userRoles = await this._userManager.GetRolesAsync(user);
            await this._userManager.RemoveFromRolesAsync(user, userRoles);

            user.PersonalInformationIsDeleted = true;
            await this.DbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new DataPersistException(
                innerException: ex,
                data: new string[] { "Delete user operation.", $"userId: {user.Id}" });
        }
    }
}