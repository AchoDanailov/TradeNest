using TradeNest.Web.Models.Order;

namespace TradeNest.Web.Models.Cart;

public class CartWithOrdersViewModel
{
    public CartViewModel? CartViewModel { get; set; }

    public IEnumerable<OrderViewModel> OrderViewModels { get; set; }
        = new HashSet<OrderViewModel>();
}