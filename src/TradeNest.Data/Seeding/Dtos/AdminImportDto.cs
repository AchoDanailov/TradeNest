using System.ComponentModel.DataAnnotations;

namespace TradeNest.Data.Seeding.Dtos;

public class AdminImportDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
}