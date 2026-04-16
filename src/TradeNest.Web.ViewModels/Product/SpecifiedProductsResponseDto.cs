namespace TradeNest.Web.ViewModels.Product;

public class SpecifiedProductsResponseDto
{
    public IEnumerable<ProductResponseDto> Products { get; set; }
        = new List<ProductResponseDto>();

    public string XsrfToken { get; set; } = null!;
}