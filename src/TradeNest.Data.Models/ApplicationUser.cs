using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[Comment("Holds User data.")]
public class ApplicationUser : IdentityUser<Guid>
{
    public virtual Admin? Admin { get; set; }
    
    public virtual Cart? Cart { get; set; }
    
    public virtual ICollection<Product> Products { get; set; }
        = new HashSet<Product>();

    public virtual ICollection<Order> Orders { get; set; }
        = new List<Order>();

    public virtual ICollection<UserWatchlistProduct> WatchlistProducts { get; set; }
        = new List<UserWatchlistProduct>();
    
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        = new List<ApplicationUserRole>();

    public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        = new List<IdentityUserClaim<Guid>>();

    public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
        = new List<IdentityUserLogin<Guid>>();

    public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
        = new List<IdentityUserToken<Guid>>();
}