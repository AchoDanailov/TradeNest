using TradeNest.Web.ViewModels.Category;

namespace TradeNest.Web.ViewModels.Product;

public class CatalogViewModel
{
    public IEnumerable<ProductViewModel> Products { get; set; }
        = new List<ProductViewModel>();
        
    public IEnumerable<AllCategoriesViewModel> Categories { get; set; }
        = new List<AllCategoriesViewModel>();

    public bool IsSearchResultSet { get; set; }
}