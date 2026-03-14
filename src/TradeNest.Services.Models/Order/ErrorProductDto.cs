using TradeNest.Services.Models.Cart.Enums;

namespace TradeNest.Services.Models.Order;

public class ErrorProductDto
{
    public ICollection<ProductErrorReason> ProductErrorReasons { get; set; }
        = new List<ProductErrorReason>();
    
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;
}