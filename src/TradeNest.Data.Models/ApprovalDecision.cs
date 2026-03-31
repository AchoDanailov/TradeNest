using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using static TradeNest.GCommon.EntityValidationConstants.ProductApprovalTicket;
using TradeNest.Data.Models.Enums;

namespace TradeNest.Data.Models;

[Owned]
public class ApprovalDecision
{
    [Required]
    [Comment("Value representing weather that product has been approved or not or is still waiting for a decision.")]
    public ApprovalStatus ApprovalStatus { get; set; }
    
    [MaxLength(DecisionJustificationMaxLengthValue)]
    [Comment("The justification for the taken decision on the product approval.")]
    public string? DecisionJustification { get; set; }
    
    [Comment("Value representing the time the ticket has been created.")]
    public DateTime LastUpdatedOn { get; set; }
    
    [Comment("Value representing the time the ticket has been processed and assigned approval status.")]
    public DateTime? TimeOfDecision { get; set; }
}