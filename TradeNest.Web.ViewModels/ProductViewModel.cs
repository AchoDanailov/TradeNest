namespace TradeNest.Web.ViewModels;

public class ProductViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string SellingPrice { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string? FrontImageUrl { get; set; }
}