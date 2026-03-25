namespace TradeNest.Web.ViewModels;

public class UpdateCartProductRequestDto
{
    public Guid CartId { get; set; }
    
    public Guid ProductId { get; set; } 
    
    public int Quantity { get; set; }
}