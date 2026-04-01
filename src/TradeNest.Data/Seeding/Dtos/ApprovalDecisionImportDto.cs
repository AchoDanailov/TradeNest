using System.ComponentModel.DataAnnotations;
using static TradeNest.GCommon.EntityValidationConstants.Product.ApprovalDecision;

namespace TradeNest.Data.Seeding.Dtos;

public class ApprovalDecisionImportDto
{
    [Required]
    public int ApprovalStatus { get; set; }
    
    [MaxLength(DecisionJustificationMaxLengthValue)]
    public string? DecisionJustification { get; set; }
    
    public DateTime? TimeOfDecision { get; set; }
}