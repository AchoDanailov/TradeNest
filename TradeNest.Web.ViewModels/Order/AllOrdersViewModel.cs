namespace TradeNest.Web.ViewModels;

public class AllOrdersViewModel
{
    public Guid Id { get; set; }
    
    public decimal? TotalPrice { get; set; }
    
    public bool IsSubmitted { get; set; }
    
    public DateOnly? SubmittedOn { get; set; }

    public IEnumerable<string> OrderProductsNames { get; set; }
        = new HashSet<string>();
}