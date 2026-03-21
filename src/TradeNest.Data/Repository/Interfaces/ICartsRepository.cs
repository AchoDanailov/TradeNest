using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface ICartsRepository : IRepository<Cart>
{
    Task<Cart?> GetUserCartWithProductsDetailsAsync(Guid userId, bool asReadOnly = false);

    Task<Cart?> GetCartWithCartProductsByUserIdAsync(Guid userId);

    Task<bool> DeleteAsync(Cart cart);
}