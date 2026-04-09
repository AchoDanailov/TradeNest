using TradeNest.Web.ViewModels.Admin;
using TradeNest.Web.ViewModels.Enums;

namespace TradeNest.Web.ViewModels.Product;

public class ApprovalDecisionViewModel
{
    public ApprovalDecisionMakerViewModel ApprovalDecisionMakerDto { get; set; } = null!;
    
    public ApprovalStatus ApprovalStatus { get; set; }

    public string? DecisionJustification { get; set; }
    
    public DateTime? TimeOfDecision { get; set; }
}