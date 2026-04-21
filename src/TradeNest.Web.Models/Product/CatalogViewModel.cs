using TradeNest.Web.Models.Category;

namespace TradeNest.Web.Models.Product;

public class CatalogViewModel
{
    public IEnumerable<ProductViewModel> Products { get; set; }
        = new List<ProductViewModel>();
        
    public IEnumerable<AllCategoriesViewModel> Categories { get; set; }
        = new List<AllCategoriesViewModel>();

    public string? SearchFilter { get; set; }
    
    public Guid? CategoryFilter { get; set; }
}