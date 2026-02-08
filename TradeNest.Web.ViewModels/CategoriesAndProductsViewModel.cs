namespace TradeNest.Web.ViewModels;

public class CategoriesAndProductsViewModel
{
    public IEnumerable<CategoryViewModel> AllCategories { get; set; }
        = new List<CategoryViewModel>();

    public IEnumerable<ProductViewModel> Products { get; set; }
        = new List<ProductViewModel>();
}