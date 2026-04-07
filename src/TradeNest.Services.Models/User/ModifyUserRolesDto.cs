using TradeNest.Services.Models.Role;

namespace TradeNest.Services.Models.User;

public class ModifyUserRolesDto
{
    public Guid Id { get; set; }

    public IEnumerable<ModifyRoleDto> AllRoles { get; set; }
        = new List<ModifyRoleDto>();
}