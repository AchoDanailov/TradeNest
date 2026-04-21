using System.ComponentModel.DataAnnotations;
using static TradeNest.GCommon.EntityValidationConstants.Product.ApprovalDecision;

namespace TradeNest.Web.Models.Product;

public class EditProductApprovalStatusRequestDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string ApprovalStatus { get; set; } = null!;

    [MaxLength(DecisionJustificationMaxLengthValue)]
    [MinLength(DecisionJustificationMinLengthValue)]
    public string? DecisionJustification { get; set; } 
}