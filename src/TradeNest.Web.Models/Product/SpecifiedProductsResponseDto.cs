namespace TradeNest.Web.Models.Product;

public class SpecifiedProductsResponseDto
{
    public IEnumerable<ProductResponseDto> Products { get; set; }
        = new List<ProductResponseDto>();

    public MetaData MetaData { get; set; } = null!;
}