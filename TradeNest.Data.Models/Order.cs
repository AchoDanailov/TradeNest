using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[Comment("Holds order's data.")]
public class Order
{
    [Key]
    [Comment("Order's primary key.")]
    public Guid Id { get; set; }

    [Required]
    [Comment("Value that represents weather the order is submitted or not.")]
    public bool IsSubmitted { get; set; } = false;
    
    [Required]
    [ForeignKey(nameof(User))]
    [Comment("Foreign key referencing the Order's creator primary key.")]
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        = new List<OrderProduct>();
}