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

    /// <summary>
    /// Assigns or removes roles provided in the <paramref name="modifyUserRolesDto.AllRoles" />
    /// collection, for the given user.
    /// </summary>
    /// <param name="adminUserId">The admin attempting the operation.</param>
    /// <param name="modifyUserRolesDto">The dto caring the user and his roles related data.</param>
    /// <returns> Task representing the asynchronous operation. </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the user or roleId are empty.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown when the <paramref name="adminUserId"/> is not an authorized administrator.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the target user or a role cannot be found.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a role already possessed or remove one not possessed by the user or
    /// user tries to delete the "Admin" role.
    /// </exception>
    /// <exception cref="DataPersistException">
    /// Thrown if the data is not successfully persisted.
    /// </exception>
    Task ModifyUserRolesAsync(Guid adminUserId, ModifyUserRolesDto modifyUserRolesDto);

    /// <summary>
    /// Removes the role with the provided <paramref name="roleToDeleteId" />.
    /// </summary>
    /// <param name="adminUserId">The admin attempting the operation.</param>
    /// <param name="roleToDeleteId">The identifier of the role to be deleted.</param>
    /// <returns> Task representing the asynchronous operation. </returns>
    /// <exception cref="ArgumentException"> Thrown when the user or roleId are empty. </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown when the <paramref name="adminUserId"/> is not an authorized administrator.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown when the target role cannot be found.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the role to deleted is the "Admin" role.
    /// </exception>
    /// <exception cref="DataPersistException">
    /// Thrown if the data is not successfully persisted.
    /// </exception>
    Task RemoveRoleAsync(Guid adminUserId, Guid roleToDeleteId);
}