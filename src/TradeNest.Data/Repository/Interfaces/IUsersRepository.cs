using System.Linq.Expressions;
using TradeNest.Data.Models;
using TradeNest.GCommon.Exceptions;

namespace TradeNest.Data.Repository.Interfaces;

// TODO: Remove the hybrid repository (IReadRepository<T>) and move to per entity repository.
// TODO: Move the Roles to a separate repository and strictly use the Identity api for working with the roles.
public interface IUsersRepository : IReadRepository<ApplicationUser>
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersWithTheirRolesAsync(
        Expression<Func<ApplicationUser, bool>>? filter = null, bool asReadOnly = false);

    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync(bool asReadOnly = false);

    Task<bool> ExistsByIdWithForgottenIncludedAsync(Guid userId, bool asReadOnly = false);

    Task<ApplicationUser?> FindByIdWithForgottenIncludedAsync(Guid userId);

    Task<bool> IsUserAdminUserByIdAsync(Guid userId);

    Task<bool> IsUserAdminUserAsync(ApplicationUser user);

    Task<bool> AddAsync(ApplicationUser user, string password);

    /// <summary>
    /// Attempts a deletion of all GDPR user data.
    /// </summary>
    /// <param name="applicationUser">The user which personal identifiable data will be deleted.</param>
    /// <returns>
    /// Task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="DataPersistException"> Thrown if the data is not successfully persisted. </exception>
    Task DeleteAsync(ApplicationUser applicationUser);

    Task<bool> AssignRolesAsync(ApplicationUser user, IEnumerable<string> roleNames);

    Task<bool> AssignRoleAsync(ApplicationUser user, string roleName);

    Task<bool> RemoveUserFromRolesAsync(ApplicationUser user, IEnumerable<string> roleNames);
    
    Task<bool> RemoveUserFromRoleAsync(ApplicationUser user, string roleName);

    Task<ApplicationRole?> FindRoleByIdAsync(Guid roleId);

    Task<bool> RemoveRoleAsync(ApplicationRole role);
}