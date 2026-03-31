using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[Comment("Represents the Admin entity.")]
public class Admin
{
    [Key]
    [Comment("Primary key of the admin entity.")]
    public Guid Id { get; set; } 
    
    [Required]
    [ForeignKey(nameof(User))]
    [Comment("Foreign key to the user that is an Admin.")]
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<Product> ProductApprovalDecisionsGiven { get; set; }
        = new List<Product>();
}