namespace TradeNest.Services.Models;

public class ProductCreateDto
{
    public string ProductName { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public int QuantityInStock { get; set; } 
    
    public decimal SellingPrice { get; set; } 

    public string? FrontImageUrl { get; set; }
    
    public string? ExtraIMagesUrls { get; set; }
    
    public decimal? CostPrice { get; set; }
    
    public bool IsEnabled { get; set; } = true;
    
    public Guid CategoryId { get; set; }
}