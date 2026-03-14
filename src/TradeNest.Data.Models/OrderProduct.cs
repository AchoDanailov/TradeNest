using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static TradeNest.GCommon.EntityValidationConstants.Product;
using static TradeNest.Data.Common.EntityModelsConstants.CommonValidationConstants;

namespace TradeNest.Data.Models;

[Comment("Represents the product state at the moment of the order being submitted.")]
public class OrderProduct
{
    [Key]
    [Comment("OrderProduct primary key.")]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(Order))]
    [Comment("Foreign key referencing the Order's primary key.")]
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    [Required]
    [MaxLength(NameMaxLengthValue)]
    [Comment("The product name at the time that the order is submitted.")]
    public string ProductNameAtOrderTime { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(OriginalProduct))]
    [Comment("Foreign key referencing the original product primary key.")]
    public Guid OriginalProductId { get; set; }
    public Product OriginalProduct { get; set; } = null!;
    
    [Required]
    [Comment("Represents the quantity of the product that was ordered.")]
    public int QuantityOrdered { get; set; }
    
    [Column(TypeName = PriceColumnDataType)]
    [Comment("Represents the cost price of the product at the moment the order is submitted.")]
    public decimal? CostPriceAtOrderTime { get; set; }
    
    [Required]
    [Column(TypeName = PriceColumnDataType)]
    [Comment("Represents the selling price of a unit of the product at the moment the order is submitted.")]
    public decimal UnitSellingPriceAtOrderTime { get; set; }
    
    [Required]
    [Column(TypeName = PriceColumnDataType)]
    [Comment($"Computed and stored in a column from {nameof(QuantityOrdered)} * {nameof(UnitSellingPriceAtOrderTime)}.")]
    public decimal TotalProductPriceAtOrderTime { get; set; }
}