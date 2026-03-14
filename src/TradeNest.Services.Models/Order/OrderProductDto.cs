namespace TradeNest.Services.Models.Order;

public class OrderProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public int QuantityOrdered { get; set; }

    public decimal UnitPriceAtOrderTime { get; set; } 

    public decimal TotalPriceAtOrderTime { get; set; } 
}