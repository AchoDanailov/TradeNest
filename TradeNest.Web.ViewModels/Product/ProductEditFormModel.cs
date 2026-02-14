using System.ComponentModel.DataAnnotations;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Image;
using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Web.ViewModels.Product;

public class ProductEditFormModel
{
    [Required]
    public string ProductId { get; set; } = null!;
    
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
    
    [Range(minimum: (double)MinCostPriceValue, maximum: Double.MaxValue)]
    public decimal? CostPrice { get; set; }
    
    [Required]
    public bool IsEnabled { get; set; }

    public List<ImageViewModel> ProductImages { get; set; }
        = new List<ImageViewModel>();
    
    [StringLength(NewImagesUrlsMaxLengthValue, MinimumLength = NewImagesUrlsMinLengthValue)]
    public string? NewImagesUrls { get; set; }

    [Required]
    public string CategoryId { get; set; } = null!;

    public IEnumerable<AllCategoriesViewModel> AllCategoriesForSelectInputFieldOptions { get; set; }
        = new List<AllCategoriesViewModel>();
}