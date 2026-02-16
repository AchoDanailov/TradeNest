namespace TradeNest.Web.ViewModels.Order;

public class OrderProductViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public int QuantityOrdered { get; set; }

    public string UnitPrice { get; set; } = null!;

    public string TotalPrice { get; set; } = null!;
}