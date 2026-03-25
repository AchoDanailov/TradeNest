using System.ComponentModel.DataAnnotations;
using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Web.ViewModels.Product;

public class ValidateProductQtyInputModel
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [Range(MinQuantityToAddToCart, MaxQuantityToAddToCart)]
    public int Quantity { get; set; }
}