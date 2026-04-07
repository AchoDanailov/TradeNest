using System.Linq.Expressions;

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
    private readonly RoleManager<ApplicationRole> _roleManager;
    
    public UsersRepository(
        TradeNestDbContext dbContext,
        UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager) 
        : base(dbContext)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersWithTheirRolesAsync(
        Expression<Func<ApplicationUser, bool>>? filter = null, bool asReadOnly = false)
    {
        IQueryable<ApplicationUser> queryable = this.DbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role);
        if (asReadOnly)
            queryable = queryable.AsNoTracking();

        if (filter != null)
            queryable = queryable.Where(filter);

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

    public async Task<bool> AssignRolesAsync(ApplicationUser user,
        IEnumerable<string> roleNames)
    {
        roleNames = roleNames.ToArray();
        
        bool result = true;
        await using IDbContextTransaction transaction 
            = await this.DbContext.Database.BeginTransactionAsync();
        
        if (roleNames.Contains("Admin"))
            result &= await this.AddAdminModelForUserAsync(user.Id);
        
        IdentityResult addToRoleResult = await this._userManager.AddToRolesAsync(user, roleNames);
        result &= addToRoleResult.Succeeded;

        await transaction.CommitAsync();
        return result;
    }

    public async Task<bool> AssignRoleAsync(ApplicationUser user, string roleName)
    {
        bool result = true;
        await using IDbContextTransaction transaction 
            = await this.DbContext.Database.BeginTransactionAsync();
        
        if(roleName == "Admin")
            result &= await this.AddAdminModelForUserAsync(user.Id);
        
        IdentityResult addToRoleResult = await this._userManager.AddToRoleAsync(user, roleName);
        result &= addToRoleResult.Succeeded;
        
        await transaction.CommitAsync();
        return result;
    }

    public async Task<bool> RemoveUserFromRolesAsync(ApplicationUser user, 
        IEnumerable<string> roleNames)
    {
        IdentityResult removeFromRolesResult 
            = await this._userManager.RemoveFromRolesAsync(user, roleNames);

        return removeFromRolesResult.Succeeded;
    }

    public async Task<bool> RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
    {
        IdentityResult addToRoleResult = await this._userManager
            .RemoveFromRoleAsync(user, roleName);
        
        return addToRoleResult.Succeeded;
    }

    public async Task<ApplicationRole?> FindRoleByIdAsync(Guid roleId)
    {
        return await this._roleManager
            .FindByIdAsync(roleId.ToString());
    }

    public async Task<bool> RemoveRoleAsync(ApplicationRole role)
    {
        bool result = true;
        await using IDbContextTransaction transaction
            = await this.DbContext.Database.BeginTransactionAsync();
        
        IEnumerable<ApplicationUser> usersInRole = await this._userManager
            .GetUsersInRoleAsync(role.Name!);
        foreach (ApplicationUser user in usersInRole)
        {
           IdentityResult removeFromRoleResult = await this._userManager
               .RemoveFromRoleAsync(user, role.Name!);
           result &= removeFromRoleResult.Succeeded;
        }

        IdentityResult deleteRoleResult = await this._roleManager.DeleteAsync(role);
        result &= deleteRoleResult.Succeeded;

        await transaction.CommitAsync();
        return result;
    }

    private async Task<bool> AddAdminModelForUserAsync(Guid userId)
    {
        Admin adminModel = new Admin() { UserId = userId };
        await this.DbContext.Admins.AddAsync(adminModel);
            
        int addedCount = await this.DbContext.SaveChangesAsync();
        return addedCount > 0;
    }
}