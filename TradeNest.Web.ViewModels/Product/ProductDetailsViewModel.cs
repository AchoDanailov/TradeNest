namespace TradeNest.Web.ViewModels.Product;

public class ProductDetailsViewModel : ProductViewModel
{
    public string Description { get; set; } = null!;

    public int QuantityInStock { get; set; } 
    
    public bool IsEnabled { get; set; } = true;

    public string OwnerName { get; set; } = null!;
    
    public bool IsOwner { get; set; }

    public IEnumerable<string> ImagesUrls { get; set; }
        = new List<string>();
}