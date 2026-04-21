using System.ComponentModel.DataAnnotations;
using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Web.Models.Cart;

public class AddProductToCartRequestDto
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    [Range(MinQuantityToAddToCart, MaxQuantityToAddToCart)]
    public int Quantity { get; set; }
}