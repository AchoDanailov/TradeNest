using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;
using TradeNest.Web.Models.Role;
using TradeNest.Web.Models.User;

namespace TradeNest.Web.Mappers.Interfaces;

public interface IUsersPresentationModelsMapper
{
    ManageUserViewModel ToManageUserViewModel(UserDto userDto);
    
    IEnumerable<ManageUserViewModel> ToManageUserViewModels(IEnumerable<UserDto> userDtos);

    RoleViewModel ToRoleViewModel(RoleDto roleDto);
    
    IEnumerable<RoleViewModel> ToRoleViewModels(IEnumerable<RoleDto> roleDtos);

    ModifyUserRolesDto FromManageUserFormModel(ManageUserRolesFormModel manageUserRolesFormModel);
}