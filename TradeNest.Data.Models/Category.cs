using System.ComponentModel.DataAnnotations;

namespace TradeNest.Data.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}