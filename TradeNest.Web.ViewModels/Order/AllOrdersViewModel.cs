namespace TradeNest.Web.ViewModels.Order;

public class AllOrdersViewModel
{
    public Guid Id { get; set; }
    
    public decimal? TotalPrice { get; set; }
    
    public bool IsSubmitted { get; set; }
    
    public string? SubmittedOn { get; set; } 

    public IEnumerable<OrderProductViewModel> OrderProducts { get; set; }
        = new HashSet<OrderProductViewModel>();
}