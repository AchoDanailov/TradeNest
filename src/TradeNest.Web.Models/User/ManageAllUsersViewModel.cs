using TradeNest.Web.Models.Role;

namespace TradeNest.Web.Models.User;

public class ManageAllUsersViewModel : ManageAllRolesViewModel
{
    public IEnumerable<ManageUserViewModel> Users { get; set; }
        = new List<ManageUserViewModel>();
}