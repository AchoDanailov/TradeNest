namespace TradeNest.Web.Models.Order;

public class OrderViewModel
{
    public Guid Id { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public DateTime SubmittedOn { get; set; } 

    public IEnumerable<OrderProductViewModel> OrderProducts { get; set; }
        = new HashSet<OrderProductViewModel>();
}