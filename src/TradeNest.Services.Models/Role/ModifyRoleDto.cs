namespace TradeNest.Services.Models.Role;

public class ModifyRoleDto : RoleDto
{
    public bool IsAssigned { get; set; }
    
    public bool IsActionTaken { get; set; }
}