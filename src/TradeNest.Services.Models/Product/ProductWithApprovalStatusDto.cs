using TradeNest.Services.Models.Enums;

namespace TradeNest.Services.Models.Product;

public class ProductWithApprovalStatusDto 
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string OwnerName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;
    
    public ApprovalStatus ApprovalStatus { get; set; }
}
