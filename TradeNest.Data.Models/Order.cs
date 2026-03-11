using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static TradeNest.Data.Common.EntityModelsConstants.CommonValidationConstants;

namespace TradeNest.Data.Models;

[Comment("Holds order's data.")]
public class Order
{
    [Key]
    [Comment("Order's primary key.")]
    public Guid Id { get; set; }

    [Required]
    [Comment("The date and time at which the order has been submitted.")]
    public DateTime SubmittedOn { get; set; }
    
    [Required]
    [Column(TypeName = PriceColumnDataType)]
    [Comment("Holds the value of the order's total price when order is submitted.")]
    public decimal TotalPrice { get; set; }
    
    [ForeignKey(nameof(User))]
    [Comment("Foreign key referencing the user that has made the order.")]
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        = new List<OrderProduct>();
}