using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Data.Seeding.Dtos;

public class CartProductImportDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(MinQuantityToAddToCart, MaxQuantityToAddToCart)]
    public int ProductQuantityAdded { get; set; }

    [Required]
    public DateTime AddedOn { get; set; }
}