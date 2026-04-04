using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.User;

namespace TradeNest.Web.ViewModels.User;

public class ManageUserRolesFormModel
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(UserNameMaxLengthValue, MinimumLength = UserNameMinLengthValue)]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(EmailMaxLengthValue, MinimumLength = EmailMinLengthValue)]
    public string Email { get; set; } = null!;

    public List<AssignRoleFormModel> AllRoles { get; set; }
        = new List<AssignRoleFormModel>();
}