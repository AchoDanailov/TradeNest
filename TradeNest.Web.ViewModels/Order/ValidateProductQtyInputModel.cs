using System.ComponentModel.DataAnnotations;
using static TradeNest.GCommon.EntityValidationConstants.Product;
using static TradeNest.GCommon.EntityValidationConstants.CommonValidationConstants;

namespace TradeNest.Web.ViewModels.Order;

public class ValidateProductQtyInputModel
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [Range(MinQuantityToAddToOrder, MaxQuantityToAddToOrder)]
    public int Quantity { get; set; }
    
    [StringLength(UrlMaxLengthValue, MinimumLength = UrlMinLengthValue)]
    public string? ReturnUrl { get; set; }
}