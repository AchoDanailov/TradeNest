namespace TradeNest.Web.ViewModels.Product;

public class ProductDetailsResponseDto
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public int QuantityInStock { get; set; }

    public string OwnerName { get; set; } = null!;

    public decimal SellingPrice { get; set; }

    public string CategoryName { get; set; } = null!;
    
    public bool IsEnabled { get; set; }

    public string Description { get; set; } = null!;

    public ApprovalDecisionResponseDto ApprovalDecision { get; set; } = null!;
    
    public IEnumerable<string> ImagesUrls { get; set; }
        = new List<string>();
}