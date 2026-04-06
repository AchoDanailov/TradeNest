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
    private readonly IUsersMapper _usersMapper;

    public UsersService(IUsersRepository usersRepository,
        IUsersMapper usersMapper, IAdminsRepository adminsRepository)
    {
        this._usersRepository = usersRepository;
        this._adminsRepository = adminsRepository;
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

        ApplicationUser? userToDelete = await this._usersRepository
            .FindByIdAsync(userToDeleteId);
        if (userToDelete == null)
            throw new ResourceNotFoundException(nameof(ApplicationUser), userToDeleteId);

        await this._usersRepository.DeleteAsync(userToDelete);
    }

    private async Task<bool> IsValidAdminId(Guid userId)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        
        return await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
    }
}