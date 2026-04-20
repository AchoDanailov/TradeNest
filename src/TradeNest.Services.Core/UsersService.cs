using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;

namespace TradeNest.Services.Core;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IAdminsRepository _adminsRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly IUsersMapper _usersMapper;

    public UsersService(
        IUsersRepository usersRepository,
        IAdminsRepository adminsRepository,
        IProductsRepository productsRepository,
        IUsersMapper usersMapper)
    {
        this._usersRepository = usersRepository;
        this._adminsRepository = adminsRepository;
        this._productsRepository = productsRepository;
        this._usersMapper = usersMapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(Guid userId)
    {
        bool isValidAdminId = await this.IsValidAdminId(userId);
        if (!isValidAdminId)
        {
            throw new UnauthorizedOperationException(userId,
                nameof(ApplicationUser), "All identifiers.");
        }

        IEnumerable<ApplicationUser> usersWithRoles = await this._usersRepository
            .GetAllUsersWithTheirRolesAsync(asReadOnly: true);

        return this._usersMapper.ToUserDtos(usersWithRoles);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync(Guid userId)
    {
        bool isValidAdminId = await this.IsValidAdminId(userId);
        if (!isValidAdminId)
        {
            throw new UnauthorizedOperationException(userId,
                nameof(ApplicationRole), "All identifiers.");
        }

        IEnumerable<ApplicationRole> allRoles = await this._usersRepository
            .GetAllRolesAsync(asReadOnly: true);

        return this._usersMapper.ToRolesDtos(allRoles);
    }

    public async Task DeleteUserByIdAsync(Guid adminUserId, Guid userToDeleteId)
    {
        bool isValidAdminId = await this.IsValidAdminId(adminUserId);
        if (!isValidAdminId)
        {
            throw new UnauthorizedOperationException(adminUserId,
                nameof(ApplicationUser), userToDeleteId);
        }

        ApplicationUser? userToDelete = await this._usersRepository.FindByIdAsync(userToDeleteId);
        if (userToDelete == null)
            throw new ResourceNotFoundException(nameof(ApplicationUser), userToDeleteId);

        await this._usersRepository.DeleteAsync(userToDelete);
    }

    public async Task ModifyUserRolesAsync(Guid adminUserId, ModifyUserRolesDto modifyUserRolesDto)
    {
        if (modifyUserRolesDto.Id == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, "userId"));

        bool isValidAdminId = await this.IsValidAdminId(adminUserId);
        if (!isValidAdminId)
        {
            throw new UnauthorizedOperationException(
                userId: adminUserId,
                resourceName: $"{nameof(ApplicationUser)}, {nameof(ApplicationRole)}",
                resourceId: $"userId: {modifyUserRolesDto.Id}");
        }

        ApplicationUser? userToModifyRoles = (await this._usersRepository
                .GetAllUsersWithTheirRolesAsync(u => u.Id == modifyUserRolesDto.Id))
            .SingleOrDefault();
        if (userToModifyRoles == null)
        {
            throw new ResourceNotFoundException(nameof(ApplicationUser),
                modifyUserRolesDto.Id);
        }

        IEnumerable<ApplicationRole> allRoles = (await this._usersRepository
                .GetAllRolesAsync(asReadOnly: true))
            .ToArray();

        ICollection<string> rolesToAssignToUser = new List<string>();
        ICollection<string> rolesToRemoveUserFrom = new List<string>();
        foreach (ModifyRoleDto modifyRoleDto in modifyUserRolesDto.AllRoles)
        {
            if (modifyRoleDto.Id == Guid.Empty)
                throw new ArgumentException(string.Format(IdCantBeEmptyMessage, "roleId"));

            if (allRoles.All(repoRole => repoRole.Id != modifyRoleDto.Id))
                throw new ResourceNotFoundException(nameof(ApplicationRole), modifyRoleDto.Id);

            if (!modifyRoleDto.IsActionTaken)
                continue;

            if (modifyRoleDto.IsAssigned)
            {
                if (userToModifyRoles.UserRoles.All(ur => ur.RoleId != modifyRoleDto.Id))
                {
                    throw new InvalidOperationException(string.Format(
                        CantRemoveRoleToNonAssignedUser, userToModifyRoles.Id, modifyRoleDto.Id));
                }

                if (modifyRoleDto.RoleName == "Admin")
                    throw new InvalidOperationException(AdminRoleCanNotBeDeletedMessage);

                rolesToRemoveUserFrom.Add(modifyRoleDto.RoleName);
            }
            else
            {
                if (userToModifyRoles.UserRoles.Any(ur => ur.RoleId == modifyRoleDto.Id))
                {
                    throw new InvalidOperationException(string.Format(
                        CantAssignRoleToAlreadyAssignedUser, userToModifyRoles.Id, modifyRoleDto.Id));
                }

                rolesToAssignToUser.Add(modifyRoleDto.RoleName);
            }
        }

        if (!rolesToAssignToUser.Any() && !rolesToRemoveUserFrom.Any())
            return;
        
        bool modifyUserRolesResult = true;
        try
        {
            if (rolesToAssignToUser.Any())
            {
                modifyUserRolesResult &= await this._usersRepository
                    .AssignRolesAsync(userToModifyRoles, rolesToAssignToUser);
            }

            if (rolesToRemoveUserFrom.Any())
            {
                modifyUserRolesResult &= await this._usersRepository
                    .RemoveUserFromRolesAsync(userToModifyRoles, rolesToRemoveUserFrom);
            }

            if (modifyUserRolesResult == false)
            {
                throw new DataPersistException(nameof(modifyUserRolesResult),
                    $"userId: {userToModifyRoles.Id}");
            }
        }
        catch (Exception ex)
        {
            throw new DataPersistException(
                innerException: ex,
                data: new string[] { nameof(modifyUserRolesResult), $"userId: {userToModifyRoles.Id}" });
        }
    }

    public async Task RemoveRoleAsync(Guid adminUserId, Guid roleToDeleteId)
    {
        if (adminUserId == Guid.Empty)
        {
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage,
                nameof(adminUserId)));
        }
        if (roleToDeleteId == Guid.Empty)
        {
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage,
                nameof(roleToDeleteId)));
        }
        
        bool isValidAdminId = await this.IsValidAdminId(adminUserId);
        if (!isValidAdminId)
        {
            throw new UnauthorizedOperationException(
                userId: adminUserId,
                resourceName: nameof(ApplicationRole),
                resourceId: roleToDeleteId);
        }

        ApplicationRole? role = await this._usersRepository
            .FindRoleByIdAsync(roleToDeleteId);
        if (role == null)
            throw new ResourceNotFoundException(nameof(ApplicationRole), roleToDeleteId);

        if (role.Name == "Admin")
            throw new InvalidOperationException(AdminRoleCanNotBeDeletedMessage);
        
        bool removeRoleResult = await this._usersRepository.RemoveRoleAsync(role);
        if (removeRoleResult == false)
        {
            throw new DataPersistException(nameof(removeRoleResult),
                nameof(removeRoleResult), $"roleId: {roleToDeleteId}");
        }
    }

    private async Task<bool> IsValidAdminId(Guid userId)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        
        return await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
    }
}