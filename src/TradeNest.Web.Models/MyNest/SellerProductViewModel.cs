using TradeNest.Web.Models.Enums;

namespace TradeNest.Web.Models.MyNest;

public class SellerProductViewModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string CategoryName { get; set; } = null!;
    
    public string? ImageUrl { get; set; }
    
    public decimal? CostPrice { get; set; }
    
    public decimal SellingPrice { get; set; }
    
    public int TimesSold { get; set; }
    
    public decimal? TotalSurplus { get; set; }
    
    public bool IsEnabled { get; set; }
    
    public ApprovalStatus ApprovalStatus { get; set; }
}