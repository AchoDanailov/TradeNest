namespace TradeNest.Web.ViewModels.User;

public class ManageUserViewModel
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public List<RoleViewModel> UserRoles { get; set; }
        = new List<RoleViewModel>();
}