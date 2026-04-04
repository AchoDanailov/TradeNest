namespace TradeNest.Web.ViewModels.User;

public class ManageAllUsersViewModel
{
    public IEnumerable<ManageUserViewModel> Users { get; set; }
        = new List<ManageUserViewModel>();

    public List<RoleViewModel> AllRoles { get; set; }
        = new List<RoleViewModel>();
    
    public string? ReturnUrl { get; set; }
}