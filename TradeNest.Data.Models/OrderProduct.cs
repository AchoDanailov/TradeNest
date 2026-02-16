using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[PrimaryKey(nameof(OrderId), nameof(ProductId))]
[Comment("Mapping entity between Orders and Products.")]
public class OrderProduct
{
    [Required]
    [ForeignKey(nameof(Order))]
    [Comment("Foreign key referencing the Order's primary key.")]
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(Product))]
    [Comment("Foreign key referencing the Product's primary key.")]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    [Required]
    [Comment("The value describes how much quantity of the given product is added in the given order.")]
    public int ProductsQuantity { get; set; }
}