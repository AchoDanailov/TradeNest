using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.Product;

namespace TradeNest.Data.Seeding.Dtos;

public class ProductImportDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MinLength(NameMinLengthValue)]
    [MaxLength(NameMaxLengthValue)]
    public string Name { get; set; } = null!;

    [Required]
    [MinLength(DescriptionMinLengthValue)]
    [MaxLength(DescriptionMaxLengthValue)]
    public string Description { get; set; } = null!;
    
    [Required]
    [Range(MinQuantityInStockValue, MaxQuantityInStockValue)]
    public int QuantityInStock { get; set; }
    
    [Range((double)MinCostPriceValue, (double)MaxCostPriceValue)]
    public decimal? CostPrice { get; set; }
    
    [Required]
    [Range((double)MinSellingPriceValue, (double)MaxSellingPriceValue)]
    public decimal SellingPrice { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
    
    public Guid? ApprovalDecisionMakerId { get; set; }

    public ApprovalDecisionImportDto ApprovalDecision { get; set; } = null!;
    
    [Required]
    public DateTime CreatedOn { get; set; }
    
    [Required]
    public bool IsEnabled { get; set; }
    
    [Required]
    public bool IsDeleted { get; set; }

    public ICollection<ImageImportDto> Images { get; set; }
        = new List<ImageImportDto>();
}