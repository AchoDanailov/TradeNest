using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.Models;

[PrimaryKey(nameof(UserId), nameof(ProductId))]
[Comment("Mapping entity representing a product in a user's wishlist.")]
public class UsersWishlistProduct
{
    [Required]
    [ForeignKey(nameof(User))]
    [Comment("Foreign key referencing the wishlist's owner primary key.")]
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(Product))]
    [Comment("Foreign key referencing the wishlist's product primary key.")]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}