using TradeNest.Services.Models.Enums;

namespace TradeNest.Services.Models.Product;

public class EditApprovalDecisionDto
{
    public ApprovalStatus ApprovalStatus { get; set; }
    
    public string? DecisionJustification { get; set; }
}