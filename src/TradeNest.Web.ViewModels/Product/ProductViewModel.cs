namespace TradeNest.Web.ViewModels.Product;

public class ProductViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal SellingPrice { get; set; } 

    public string CategoryName { get; set; } = null!;

    public string? FrontImageUrl { get; set; }
    
    public Guid OwnerId { get; set; }
    
    public bool IsEnabled { get; set; }
}