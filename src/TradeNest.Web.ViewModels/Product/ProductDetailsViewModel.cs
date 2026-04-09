namespace TradeNest.Web.ViewModels.Product;

public class ProductDetailsViewModel : ProductViewModel
{
    public string Description { get; set; } = null!;

    public int QuantityInStock { get; set; }

    public ApprovalDecisionViewModel ApprovalDecision { get; set; } = null!;
    
    public decimal? CostPrice { get; set; }

    public string OwnerName { get; set; } = null!;
    
    public bool IsOwner { get; set; }

    public string ReturnUrl { get; set; } = null!;

    public IEnumerable<string> ImagesUrls { get; set; }
        = new List<string>();
}