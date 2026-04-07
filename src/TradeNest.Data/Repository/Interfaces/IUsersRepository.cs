using System.Linq.Expressions;
using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IUsersRepository : IReadRepository<ApplicationUser>
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersWithTheirRolesAsync(
        Expression<Func<ApplicationUser, bool>>? filter = null, bool asReadOnly = false);

    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync(bool asReadOnly = false);

    Task<bool> ExistsByIdWithForgottenIncludedAsync(Guid userId, bool asReadOnly = false);

    Task<ApplicationUser?> FindByIdWithForgottenIncludedAsync(Guid userId);
    
    Task<bool> AddAsync(ApplicationUser user, string password);

    Task DeleteAsync(ApplicationUser applicationUser);

    Task<bool> AssignRolesAsync(ApplicationUser user, IEnumerable<string> roleNames);

    Task<bool> AssignRoleAsync(ApplicationUser user, string roleName);

    Task<bool> RemoveUserFromRolesAsync(ApplicationUser user, IEnumerable<string> roleNames);
    
    Task<bool> RemoveUserFromRoleAsync(ApplicationUser user, string roleName);

    Task<ApplicationRole?> FindRoleByIdAsync(Guid roleId);

    Task<bool> RemoveRoleAsync(ApplicationRole role);
}
