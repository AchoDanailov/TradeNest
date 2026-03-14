using TradeNest.Web.ViewModels.Order;

namespace TradeNest.Web.ViewModels.Cart;

public class CartWithOrdersViewModel
{
    public CartViewModel? CartViewModel { get; set; }

    public IEnumerable<OrderViewModel> OrderViewModels { get; set; }
        = new HashSet<OrderViewModel>();
}