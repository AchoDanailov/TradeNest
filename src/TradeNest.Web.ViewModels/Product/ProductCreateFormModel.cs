using System.ComponentModel.DataAnnotations;

using TradeNest.Web.ViewModels.Category;
using static TradeNest.GCommon.EntityValidationConstants.Product;
using static TradeNest.GCommon.EntityValidationConstants.CommonValidationConstants;

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
    
    [Range(minimum: (double)MinCostPriceValue, maximum: Double.MaxValue)]
    public decimal? CostPrice { get; set; }
    
    [Required]
    public bool IsEnabled { get; set; }

    [StringLength(UrlMaxLengthValue, MinimumLength = UrlMinLengthValue)]
    [DataType(DataType.ImageUrl)]
    public string? FrontImageUrl { get; set; } 
    
    [StringLength(ExtraImagesUrlsMaxLengthValue, MinimumLength = ExtraImagesUrlsMinLengthValue)]
    public string? ExtraImagesUrls { get; set; }

    [Required(ErrorMessage = "The Category field is required.")]
    public Guid CategoryId { get; set; }

    public string ReturnUrl { get; set; } = null!;

    public IEnumerable<AllCategoriesViewModel> AllCategories { get; set; }
        = new List<AllCategoriesViewModel>();
}