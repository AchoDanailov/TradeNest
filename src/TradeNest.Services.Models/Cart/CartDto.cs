namespace TradeNest.Services.Models.Cart;

public class CartDto
{
    public Guid CartId { get; set; }
    
    public decimal TotalPrice { get; set; }

    public IEnumerable<CartProductDto> CartProducts { get; set; }
        = new List<CartProductDto>();
}