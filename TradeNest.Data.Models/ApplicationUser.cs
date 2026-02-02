using Microsoft.AspNetCore.Identity;

namespace TradeNest.Data.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public virtual ICollection<Product> Products { get; set; }
        = new HashSet<Product>();

    public virtual ICollection<Order> Orders { get; set; }
        = new List<Order>();
}