using TradeNest.Web.ViewModels.Role;

namespace TradeNest.Web.ViewModels.User;

public class ManageAllUsersViewModel : ManageAllRolesViewModel
{
    public IEnumerable<ManageUserViewModel> Users { get; set; }
        = new List<ManageUserViewModel>();
}