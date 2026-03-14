using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[Comment("Holds User data.")]
public class ApplicationUser : IdentityUser<Guid>
{
    [Comment("One to one relation with Cart. Principal.")]
    public Cart? Cart { get; set; }
    
    public virtual ICollection<Product> Products { get; set; }
        = new HashSet<Product>();

    public virtual ICollection<Order> Orders { get; set; }
        = new List<Order>();

    public virtual ICollection<UserWatchlistProduct> WatchlistProducts { get; set; }
        = new List<UserWatchlistProduct>();
}