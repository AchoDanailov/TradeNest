using System.ComponentModel.DataAnnotations;

namespace TradeNest.Data.Seeding.Dtos;

public class CartImportDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid CartOwnerId { get; set; }

    public ICollection<CartProductImportDto> CartProducts { get; set; }
        = new List<CartProductImportDto>();
}