using System.ComponentModel.DataAnnotations;

using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Image;
using static TradeNest.GCommon.EntityValidationConstants.Product;
using static TradeNest.GCommon.FormsInvalidInputsNotificationMessages.Product;

namespace TradeNest.Web.ViewModels.Product;

public class ProductEditFormModel
{
    [Required(ErrorMessage = ProductIdRequired)]
    public Guid ProductId { get; set; } 
    
    [Required(ErrorMessage = ProductNameRequired)]
    [StringLength(NameMaxLengthValue, MinimumLength = NameMinLengthValue, ErrorMessage = ProductNameLength)]
    public string ProductName { get; set; } = null!;

    [Required(ErrorMessage = DescriptionRequired)]
    [StringLength(DescriptionMaxLengthValue, MinimumLength = DescriptionMinLengthValue, ErrorMessage = DescriptionLength)]
    public string Description { get; set; } = null!;

    [Required]
    [Range(MinQuantityInStockValue, MaxQuantityInStockValue, ErrorMessage = QuantityInStockRange)]
    public int QuantityInStock { get; set; }
    
    [Required(ErrorMessage = SellingPriceRequired)]
    [Range(minimum: (double)MinSellingPriceValue, maximum: Double.MaxValue, ErrorMessage = SellingPriceRange)]
    public decimal SellingPrice { get; set; }
    
    [Range(minimum: (double)MinCostPriceValue, maximum: Double.MaxValue, ErrorMessage = CostPriceRange)]
    public decimal? CostPrice { get; set; }
    
    [Required]
    public bool IsEnabled { get; set; }

    public List<ImageViewModel> ProductImages { get; set; }
        = new List<ImageViewModel>();
    
    [StringLength(NewImagesUrlsMaxLengthValue, MinimumLength = NewImagesUrlsMinLengthValue, ErrorMessage = NewImagesUrlsLength)]
    public string? NewImagesUrls { get; set; }

    [Required(ErrorMessage = CategoryRequired)]
    public Guid CategoryId { get; set; }

    public string ReturnUrl { get; set; } = null!;

    public IEnumerable<AllCategoriesViewModel> AllCategories { get; set; }
        = new List<AllCategoriesViewModel>();
}