using System.ComponentModel.DataAnnotations;
using static TradeNest.GCommon.EntityValidationConstants.Role;

namespace TradeNest.Web.Models.Role;

public class AssignRoleFormModel
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(NameMaxLengthValue)]
    public string RoleName { get; set; } = null!;
    
    [Required]
    public bool IsAssigned { get; set; }
    
    [Required]
    public bool IsActionTaken { get; set; }
}