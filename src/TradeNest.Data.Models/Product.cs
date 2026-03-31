using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Models.Enums;
using static TradeNest.GCommon.EntityValidationConstants.Product;
using static TradeNest.Data.Common.EntityModelsConstants.CommonValidationConstants;

namespace TradeNest.Data.Models;

[Comment("Holds product's data.")]
public class Product
{
    [Key]
    [Comment("Product's primary key.")]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(NameMaxLengthValue)]
    [Comment("Product's name.")]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(DescriptionMaxLengthValue)]
    [Comment("Product's description.")]
    public string Description { get; set; } = null!;
    
    [Required]
    [Comment("The quantity of the product that is available in stock.")]
    public int QuantityInStock { get; set; }
    
    [Column(TypeName = PriceColumnDataType)]
    [Comment("The price cost of attaining/producing the product. Is for user statistics. Nullable.")]
    public decimal? CostPrice { get; set; }

    [Required]
    [Column(TypeName = PriceColumnDataType)]
    [Comment("The price the product is being sold at.")]
    public decimal SellingPrice { get; set; }

    [Required]
    [Comment("Date of creating. Has default universal time set on record insertion to date and time of insertion.")]
    public DateTime CreatedOn { get; set; }

    [Required]
    [Comment("Value is used to show weather the product is enabled or disabled for selling.")]
    public bool IsEnabled { get; set; } = true;
    
    [Required]
    [Comment("Value representing weather the product has been approved or not or is still waiting for approval.")]
    public ApprovalStatus ApprovalStatus { get; set; }

    [Required]
    [Comment("Value is used to show weather the product deleted.")]
    public bool IsDeleted { get; set; } = false;

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
    
    [ForeignKey(nameof(AdminToApprove))]
    [Comment("The foreign key to the admin entity that has the product listed for approval.")]
    public Guid? AdminId { get; set; }
    public virtual Admin? AdminToApprove { get; set; }

    [ForeignKey(nameof(Owner))]
    [Comment("Foreign key referencing the product's owner primary key.")]
    public Guid OwnerId { get; set; }
    public virtual ApplicationUser Owner { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(Category))]
    [Comment("Foreign key referencing the product's category primary key.")]
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; }
        = new HashSet<Image>();

    public virtual ICollection<OrderProduct> SoldProducts { get; set; }
        = new List<OrderProduct>();
    
    public virtual ICollection<CartProduct> ProductCarts { get; set; }
        = new List<CartProduct>();

    public virtual ICollection<UserWatchlistProduct> ProductWatchlists { get; set; }
        = new List<UserWatchlistProduct>();
}