using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.Role;

namespace TradeNest.Web.ViewModels.User;

public class AssignRoleFormModel
{
    [Required]
    public string Id { get; set; } = null!;
    
    [Required]
    [MaxLength(NameMaxLengthValue)]
    public string RoleName { get; set; } = null!;
    
    [Required]
    public bool IsAssigned { get; set; }
    
    [Required]
    public bool IsActionTaken { get; set; }
}