using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static TradeNest.GCommon.EntityValidationConstants.Category;

namespace TradeNest.Data.Models;

[Comment("Holds category data.")]
public class Category
{
    [Key]
    [Comment("Category's primary key.")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(NameMaxLengthValue)]
    [Comment("Category's name.")]
    public string Name { get; set; } = null!;

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}