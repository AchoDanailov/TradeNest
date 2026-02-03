using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TradeNest.GCommon.EntityValidationConstants.Image;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

// TODO: Use a file server
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
    [ForeignKey(nameof(Product))]
    [Comment("Foreign key referencing the image's product primary key.")]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}