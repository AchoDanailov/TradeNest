namespace TradeNest.Services.Models;

public class ProductDetailsDto : ProductDto
{
    public string Description { get; set; } = null!;

    public int QuantityInStock { get; set; } 
    
    public bool IsEnabled { get; set; } = true;

    public string OwnerName { get; set; } = null!;
    
    public bool IsOwner { get; set; }

    public IEnumerable<string> ImagesUrls { get; set; }
        = new List<string>();
}