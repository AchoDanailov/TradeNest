namespace TradeNest.Web.Models.Cart;

public class CartViewModel
{
    public Guid CartId { get; set; }

    public decimal TotalPrice { get; set; }

    public IEnumerable<CartProductViewModel> CartProducts { get; set; }
        = new List<CartProductViewModel>();
}