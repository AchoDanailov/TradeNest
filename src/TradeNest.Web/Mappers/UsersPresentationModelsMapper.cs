using Riok.Mapperly.Abstractions;

using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.ViewModels.Role;
using TradeNest.Web.ViewModels.User;

namespace TradeNest.Web.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UsersPresentationModelsMapper : IUsersPresentationModelsMapper
{
    [MapPropertyFromSource(nameof(ManageUserViewModel.UserRoles), Use = nameof(MapUserRolesOrderedByRoleName))]
    public partial ManageUserViewModel ToManageUserViewModel(UserDto userDto);

    public partial IEnumerable<ManageUserViewModel> ToManageUserViewModels(IEnumerable<UserDto> userDtos);

    public partial RoleViewModel ToRoleViewModel(RoleDto roleDto);

    public partial IEnumerable<RoleViewModel> ToRoleViewModels(IEnumerable<RoleDto> roleDtos);
    
    public partial ModifyUserRolesDto FromManageUserFormModel(ManageUserRolesFormModel manageUserRolesFormModel);

    private static List<RoleViewModel> MapUserRolesOrderedByRoleName(UserDto userDto)
    {
        return userDto.UserRoles
            .Select(ur => new RoleViewModel()
            {
                Id = ur.Id.ToString(),
                RoleName = ur.RoleName
            })
            .OrderBy(r => r.RoleName)
            .ToList();
    }
}