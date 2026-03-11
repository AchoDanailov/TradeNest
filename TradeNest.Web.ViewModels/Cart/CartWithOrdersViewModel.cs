using TradeNest.Web.ViewModels.Order;

namespace TradeNest.Web.ViewModels.Cart;

public class CartWithOrdersViewModel
{
    public IEnumerable<ErrorProductViewModel> ErrorProductsViewModels { get; set; }
        = new List<ErrorProductViewModel>();
    
    public CartViewModel? CartViewModel { get; set; }

    public IEnumerable<OrderViewModel> OrderViewModels { get; set; }
        = new HashSet<OrderViewModel>();
}