using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.User;

namespace TradeNest.Data.Seeding.Dtos;

public class UserImportDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MinLength(UserNameMinLengthValue)]
    [MaxLength(UserNameMaxLengthValue)]
    public string UserName { get; set; } = null!;

    [Required]
    [MinLength(EmailMinLengthValue)]
    [MaxLength(EmailMaxLengthValue)]
    public string Email { get; set; } = null!;
    
    [Required]
    [MinLength(PasswordMinLengthValue)]
    [MaxLength(PasswordMaxLengthValue)]
    public string Password { get; set; } = null!;
}