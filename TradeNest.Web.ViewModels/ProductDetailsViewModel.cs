namespace TradeNest.Web.ViewModels;

public class ProductDetailsViewModel : ProductViewModel
{
    public string Description { get; set; } = null!;

    public int QuantityInStock { get; set; } 
    
    public bool IsEnabled { get; set; } = true;

    public string Owner { get; set; } = null!;

    public IEnumerable<string> ImagesUrls { get; set; }
        = new List<string>();
}