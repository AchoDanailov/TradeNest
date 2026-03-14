using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[Comment("Holds Cart data.")]
public class Cart
{
    [Key]
    [Comment("Cart's primary key.")]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(CartOwner))]
    [Comment("One to one relation with User. Dependant.")]
    public Guid CartOwnerId { get; set; }
    public ApplicationUser CartOwner { get; set; } = null!;

    public ICollection<CartProduct> CartProducts { get; set; }
        = new List<CartProduct>();
}