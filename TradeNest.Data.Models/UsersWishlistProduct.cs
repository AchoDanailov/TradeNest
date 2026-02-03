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
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

}