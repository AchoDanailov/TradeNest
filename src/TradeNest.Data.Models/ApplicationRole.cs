using Microsoft.AspNetCore.Identity;

namespace TradeNest.Data.Models;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
    }
    
    public ApplicationRole(string roleName)
        : base(roleName: roleName)
    {
    }
    
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        = new List<ApplicationUserRole>();
}