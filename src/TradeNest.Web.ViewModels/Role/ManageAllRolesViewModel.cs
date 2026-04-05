namespace TradeNest.Web.ViewModels.Role;

public class ManageAllRolesViewModel
{
    public IEnumerable<RoleViewModel> AllRoles { get; set; }
        = new List<RoleViewModel>();
    
    public string? ReturnUrl { get; set; }
}