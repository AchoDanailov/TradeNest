using System.ComponentModel.DataAnnotations;
using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Web.Models.Cart;

public class UpdateCartProductRequestDto
{
    [Required]
    public Guid CartId { get; set; }
    
    [Required]
    public Guid ProductId { get; set; } 
    
    [Range(MinQuantityToAddToCart, MaxQuantityToAddToCart)]
    public int Quantity { get; set; }
}