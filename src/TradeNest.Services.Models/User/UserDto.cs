using TradeNest.Services.Models.Role;

namespace TradeNest.Services.Models.User;

public class UserDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual List<RoleDto> UserRoles { get; set; }
        = new List<RoleDto>();
}