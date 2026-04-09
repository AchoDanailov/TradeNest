namespace TradeNest.Services.Models.Product;

public class ProductDetailsDto : ProductDto
{
    public string Description { get; set; } = null!;

    public int QuantityInStock { get; set; } 
    
    public decimal? CostPrice { get; set; }

    public ApprovalDecisionDto ApprovalDecision { get; set; } = null!;

    public string OwnerName { get; set; } = null!;
    
    public bool IsOwner { get; set; }

    public IEnumerable<string> ImagesUrls { get; set; }
        = new List<string>();
}