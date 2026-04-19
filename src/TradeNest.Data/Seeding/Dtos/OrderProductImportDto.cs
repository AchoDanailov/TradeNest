using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Data.Seeding.Dtos;

public class OrderProductImportDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MinLength(NameMinLengthValue)]
    [MaxLength(NameMaxLengthValue)]
    public string ProductNameAtOrderTime { get; set; } = null!;

    [Required]
    public Guid OriginalProductId { get; set; }

    [Required]
    [Range(MinQuantityToAddToCart, MaxQuantityToAddToCart)]
    public int QuantityOrdered { get; set; }

    public decimal? CostPriceAtOrderTime { get; set; }

    [Required]
    [Range((double)MinSellingPriceValue, (double)MaxSellingPriceValue)]
    public decimal UnitSellingPriceAtOrderTime { get; set; }
}