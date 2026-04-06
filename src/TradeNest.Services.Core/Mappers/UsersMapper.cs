using Riok.Mapperly.Abstractions;

using TradeNest.Data.Models;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;

namespace TradeNest.Services.Core.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UsersMapper : IUsersMapper
{
    public partial IEnumerable<UserDto> ToUserDtos(IEnumerable<ApplicationUser> applicationUsers);
    
    [MapProperty(nameof(ApplicationUser.UserName), nameof(UserDto.Username))]
    [MapPropertyFromSource(nameof(UserDto.UserRoles), Use = nameof(MapUserRoles))]
    public partial UserDto ToUserDto(ApplicationUser applicationUser);

    [MapProperty(nameof(ApplicationRole.Name), nameof(RoleDto.RoleName))]
    public partial RoleDto ToRoleDto(ApplicationRole role);

    public partial IEnumerable<RoleDto> ToRolesDtos(IEnumerable<ApplicationRole> applicationRoles);

    private static List<RoleDto> MapUserRoles(ApplicationUser applicationUser)
    {
        return applicationUser.UserRoles
            .Select(ur => new RoleDto
            {
                Id = ur.RoleId,
                RoleName = ur.Role.Name ?? throw new ArgumentNullException(
                    paramName: nameof(RoleDto.RoleName),
                    message: "RoleName could not be mapped. Property of Role Name was null. Please check if you are loading the roles before calling the mapper method.")
            })
            .ToList();
    }
}