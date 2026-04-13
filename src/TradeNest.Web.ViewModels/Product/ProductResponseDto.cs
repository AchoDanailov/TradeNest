namespace TradeNest.Web.ViewModels.Product;

public class ProductResponseDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public string OwnerName { get; set; } = null!;

    public string ApprovalStatus { get; set; } = null!;
}