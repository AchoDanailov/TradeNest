using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;

namespace TradeNest.Services.Core.Interfaces;

public interface IUsersService
{
    /// <summary>
    /// Retrieves all users with their assigned roles.
    /// </summary>
    /// <param name="userId">The admin user's identifier requesting the data.</param>
    /// <returns>Task that returns all users.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId" /> is with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the user with the provided <paramref name="userId" /> is not an admin.
    /// </exception>
    Task<IEnumerable<UserDto>> GetAllUsersAsync(Guid userId);

    /// <summary>
    /// Retrieves all roles.
    /// </summary>
    /// <param name="userId">The admin user's identifier requesting the data.</param>
    /// <returns>Task that returns all roles.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided userId is with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the user with the provided <paramref name="userId" /> is not an admin.
    /// </exception>
    Task<IEnumerable<RoleDto>> GetAllRolesAsync(Guid userId);

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="adminUserId">The user attempting the operation.</param>
    /// <param name="userToDeleteId">The user to be deleted.</param>
    /// <returns>Task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided userId is with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the user with the provided <paramref name="adminUserId" /> is not an admin.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if the user with the provided <paramref name="userToDeleteId" /> does not exist.
    /// </exception>
    Task DeleteUserByIdAsync(Guid adminUserId, Guid userToDeleteId);
}