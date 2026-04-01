using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.Category;

namespace TradeNest.Data.Seeding.Dtos;

public class CategoryImportDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MinLength(NameMinLengthValue)]
    [MaxLength(NameMaxLengthValue)]
    public string Name { get; set; } = null!;
}