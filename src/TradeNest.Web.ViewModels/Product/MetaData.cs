namespace TradeNest.Web.ViewModels.Product;

public class MetaData
{
    public int TotalSpecifiedProductsCount { get; set; }
    
    public string XsrfToken { get; set; } = null!;
}