namespace TradeNest.Web.Models.MyNest;

public class SellerDashboardViewModel
{
    public int TotalSales { get; set; }
    
    public decimal TotalRevenue { get; set; }
    
    public decimal TotalSurplus { get; set; }

    public bool HasProductsWithoutCostPrice { get; set; }
    
    public IEnumerable<SellerProductViewModel> ApprovedProducts { get; set; } 
        = new List<SellerProductViewModel>();
    
    public IEnumerable<SellerProductViewModel> NonApprovedProducts { get; set; } 
        = new List<SellerProductViewModel>();
}