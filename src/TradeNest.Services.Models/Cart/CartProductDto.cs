namespace TradeNest.Services.Models.Cart;

public class CartProductDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public int QuantityAdded { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public DateTime AddedOn { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public bool IsEnoughQtyLeft { get; set; }
}