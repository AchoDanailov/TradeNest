namespace TradeNest.Web.ViewModels.Product;

public class CatalogProductsAndCategoriesViewModel
{
    public IEnumerable<ProductViewModel> Products { get; set; }
        = new List<ProductViewModel>();
        
    public IEnumerable<string> CategoriesNames { get; set; }
        = new List<string>();

    public bool IsSearchResultSet { get; set; }
}