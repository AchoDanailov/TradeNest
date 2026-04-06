using TradeNest.Data.Models;
using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;

namespace TradeNest.Services.Core.Mappers.Interfaces;

public interface IUsersMapper
{
    UserDto ToUserDto(ApplicationUser applicationUser);
    
    IEnumerable<UserDto> ToUserDtos(IEnumerable<ApplicationUser> applicationUsers);
    
    RoleDto ToRoleDto(ApplicationRole role);

    IEnumerable<RoleDto> ToRolesDtos(IEnumerable<ApplicationRole> applicationRoles);
}