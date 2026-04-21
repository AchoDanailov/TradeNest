using TradeNest.Web.Models.Admin;
using TradeNest.Web.Models.Enums;

namespace TradeNest.Web.Models.Product;

public class ApprovalDecisionViewModel
{
    public ApprovalDecisionMakerViewModel ApprovalDecisionMakerDto { get; set; } = null!;
    
    public ApprovalStatus ApprovalStatus { get; set; }

    public string? DecisionJustification { get; set; }
    
    public DateTime? TimeOfDecision { get; set; }
}