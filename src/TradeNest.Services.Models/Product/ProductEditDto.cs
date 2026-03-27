using TradeNest.Services.Models.Image;

namespace TradeNest.Services.Models.Product;

public class ProductEditDto : ProductDto
{
    public decimal? CostPrice { get; set; }
    
    public string Description { get; set; } = null!;

    public int QuantityInStock { get; set; } 

    public IEnumerable<ImageDto> ProductImages { get; set; }
        = new List<ImageDto>();
    
    public string? NewImagesUrls { get; set; }
    
    public Guid CategoryId { get; set; }
}