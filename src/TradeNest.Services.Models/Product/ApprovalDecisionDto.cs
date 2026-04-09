using TradeNest.Services.Models.Admin;
using TradeNest.Services.Models.Enums;

namespace TradeNest.Services.Models.Product;

public class ApprovalDecisionDto
{
    public ApprovalDecisionMakerDto ApprovalDecisionMakerDto { get; set; } = null!;
    
    public ApprovalStatus ApprovalStatus { get; set; }

    public string? DecisionJustification { get; set; }
    
    public DateTime? TimeOfDecision { get; set; }
}