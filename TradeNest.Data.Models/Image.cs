using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TradeNest.GCommon.EntityValidationConstants.Image;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[Comment("Holds Image data.")]
public class Image
{
    [Key]
    [Comment("Image's primary key")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(UrlMaxLengthValue)]
    [Comment("Image's Url")]
    public string Url { get; set; } = null!;

    [Required]
    [Comment("Value represents weather the image is used as a front image for the product or not.")]
    public bool IsFrontImage { get; set; } = false;
    
    [Required]
    [ForeignKey(nameof(Product))]
    [Comment("Foreign key referencing the image's product primary key.")]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}