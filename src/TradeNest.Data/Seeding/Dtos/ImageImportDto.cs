using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.CommonValidationConstants;

namespace TradeNest.Data.Seeding.Dtos;

public class ImageImportDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MinLength(UrlMinLengthValue)]
    [MaxLength(UrlMaxLengthValue)]
    public string Url { get; set; } = null!;
    
    [Required]
    public bool IsFrontImage { get; set; }
}