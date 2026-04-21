using System.ComponentModel.DataAnnotations;
using TradeNest.Web.Models.Category;
using static TradeNest.GCommon.EntityValidationConstants.Product;
using static TradeNest.GCommon.EntityValidationConstants.CommonValidationConstants;
using static TradeNest.GCommon.FormsInvalidInputsNotificationMessages.Product;

namespace TradeNest.Web.Models.Product;

public class ProductCreateFormModel
{
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
    [Range(minimum: (double)MinSellingPriceValue, maximum: (double)MaxSellingPriceValue, ErrorMessage = SellingPriceRange)]
    public decimal SellingPrice { get; set; }
    
    [Range(minimum: (double)MinCostPriceValue, maximum: (double)MaxCostPriceValue, ErrorMessage = CostPriceRange)]
    public decimal? CostPrice { get; set; }
    
    [Required]
    public bool IsEnabled { get; set; }

    [StringLength(UrlMaxLengthValue, MinimumLength = UrlMinLengthValue, ErrorMessage = FrontImageUrlLength)]
    [DataType(DataType.ImageUrl)]
    public string? FrontImageUrl { get; set; } 
    
    [StringLength(ExtraImagesUrlsMaxLengthValue, MinimumLength = ExtraImagesUrlsMinLengthValue, ErrorMessage = ExtraImagesUrlsLength)]
    public string? ExtraImagesUrls { get; set; }

    [Required(ErrorMessage = CategoryRequired)]
    public Guid CategoryId { get; set; }
    
    public string? ReturnUrl { get; set; } 

    public IEnumerable<AllCategoriesViewModel> AllCategories { get; set; }
        = new List<AllCategoriesViewModel>();
}