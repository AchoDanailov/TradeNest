namespace TradeNest.Web.ViewModels.Product;

public class ProductDetailsResponseDto
{
    public string Name { get; set; } = null!;
    
    public int QuantityInStock { get; set; }

    public string OwnerName { get; set; } = null!;

    public decimal SellingPrice { get; set; }

    public string CategoryName { get; set; } = null!;
    
    public bool IsEnabled { get; set; }
}