namespace TradeNest.Web.ViewModels.Cart;

public class CartProductViewModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public Guid OriginalProductId { get; set; }

    public int QuantityAdded { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public bool IsEnoughQtyLeft { get; set; }
}