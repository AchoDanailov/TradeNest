using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.User;
using TradeNest.Web.ViewModels.Role;

namespace TradeNest.Web.ViewModels.User;

public class ManageUserRolesFormModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(UserNameMaxLengthValue, MinimumLength = UserNameMinLengthValue)]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(EmailMaxLengthValue, MinimumLength = EmailMinLengthValue)]
    public string Email { get; set; } = null!;
    
    public string? ReturnUrl { get; set; }

    public List<AssignRoleFormModel> AllRoles { get; set; }
        = new List<AssignRoleFormModel>();
}