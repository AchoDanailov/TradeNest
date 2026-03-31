using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[Comment("Entity representing an application admin. 1 to 1 with User.")]
public class Admin
{
    [Key]
    [Comment("The primary key of the Admin entity.")]
    public Guid Id { get; set; }
    
    [Required]
    [Comment("Foreign key to the user that is an admin. 1 to 1.")]
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    
    public virtual ICollection<Product> ProductsToApprove { get; set; }
        = new List<Product>();
}