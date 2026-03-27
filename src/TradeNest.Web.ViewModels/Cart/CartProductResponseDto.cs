namespace TradeNest.Web.ViewModels.Cart;

public class CartProductResponseDto
{
    public string Name { get; set; } = null!;
    
    public int QuantityAdded { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public DateTime AddedOn { get; set; }
}