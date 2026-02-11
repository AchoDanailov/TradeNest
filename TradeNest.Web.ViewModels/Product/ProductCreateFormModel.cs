using System.ComponentModel.DataAnnotations;
using TradeNest.Web.ViewModels.Category;
using static TradeNest.GCommon.EntityValidationConstants.Product;
using static TradeNest.GCommon.EntityValidationConstants.Image;

namespace TradeNest.Web.ViewModels.Product;

public class ProductCreateFormModel
{
    [Required]
    [StringLength(NameMaxLengthValue, MinimumLength = NameMinLengthValue)]
    public string ProductName { get; set; } = null!;

    [Required]
    [StringLength(DescriptionMaxLengthValue, MinimumLength = DescriptionMinLengthValue)]
    public string Description { get; set; } = null!;

    [Required]
    [Range(MinQuantityInStockValue, MaxQuantityInStockValue)]
    public int QuantityInStock { get; set; }
    
    [Required]
    [Range(minimum: (double)MinSellingPriceValue, maximum: Double.MaxValue)]
    public decimal SellingPrice { get; set; }
    
    public decimal? CostPrice { get; set; }
    
    [Required]
    public bool IsEnabled { get; set; }

    [StringLength(UrlMaxLengthValue, MinimumLength = UrlMinLengthValue)]
    [DataType(DataType.ImageUrl)]
    public string? FrontImageUrl { get; set; } 

    [Required]
    public string CategoryId { get; set; } = null!;

    public IEnumerable<AllCategoriesViewModel> AllCategoriesForSelectInputFieldOptions { get; set; }
        = new List<AllCategoriesViewModel>();
}