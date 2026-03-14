using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[PrimaryKey(nameof(ProductId), nameof(CartId))]
[Comment("Mapping entity between Cart and Products - represents product added to cart.")]
public class CartProduct
{
    [Required]
    [ForeignKey(nameof(Product))]
    [Comment("Foreign key referencing the product's primary key.")]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(Cart))]
    [Comment("Foreign key referencing the cart's primary key.")]
    public Guid CartId { get; set; }
    public virtual Cart Cart { get; set; } = null!;
    
    [Required]
    [Comment("The value describes how much quantity of the given product is added in the given Cart.")]
    public int ProductQuantityAdded { get; set; }
    
    [Required]
    [Comment("The date and time that the product was added to the cart.")]
    public DateTime AddedOn { get; set; }
}