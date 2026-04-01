using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface ICartsRepository : IReadRepository<Cart>
{
    Task<Cart?> GetCartWithProductsDetailsAsync(Guid cartId, bool asReadOnly = false);
    
    Task<Cart?> GetUserCartWithProductsDetailsAsync(Guid userId, bool asReadOnly = false);

    Task<Cart?> GetCartWithCartProductsByUserIdAsync(Guid userId);

    Task<bool> AddAsync(Cart cart);

    Task<bool> UpdateAsync(Cart cart);

    Task<bool> DeleteAsync(Cart cart);
}