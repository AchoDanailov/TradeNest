namespace TradeNest.Web.Models.Product;

public class ApprovalDecisionResponseDto
{
    public string? ApprovalDecisionMakerUsername { get; set; }

    public string ApprovalStatus { get; set; } = null!;
    
    public string? DecisionJustification { get; set; }
    
    public DateTime? TimeOfDecision { get; set; }
}