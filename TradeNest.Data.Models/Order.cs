using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TradeNest.GCommon.EntityValidationConstants.CommonValidationConstants;
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
    
    [Column(TypeName = PriceColumnDataType)]
    [Comment("Holds the value of the order's total price when order is submitted.")]
    public decimal? TotalPrice { get; set; }
    
    [ForeignKey(nameof(User))]
    [Comment("Foreign key referencing the user making the order.")]
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        = new List<OrderProduct>();
}