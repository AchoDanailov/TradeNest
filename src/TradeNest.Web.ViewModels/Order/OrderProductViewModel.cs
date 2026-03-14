namespace TradeNest.Web.ViewModels.Order;

public class OrderProductViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public int QuantityOrdered { get; set; }

    public decimal UnitPrice { get; set; } 

    public decimal TotalPrice { get; set; } 
}