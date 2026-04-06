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
    public partial ManageUserViewModel ToManageUserViewModel(UserDto userDto);

    public partial IEnumerable<ManageUserViewModel> ToManageUserViewModels(IEnumerable<UserDto> userDtos);

    public partial RoleViewModel ToRoleViewModel(RoleDto roleDto);

    public partial IEnumerable<RoleViewModel> ToRoleViewModels(IEnumerable<RoleDto> roleDto);
}