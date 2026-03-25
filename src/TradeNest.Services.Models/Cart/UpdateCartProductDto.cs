namespace TradeNest.Services.Models.Cart;

public class UpdateCartProductDto
{
    public Guid CartId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public int Quantity { get; set; }
}