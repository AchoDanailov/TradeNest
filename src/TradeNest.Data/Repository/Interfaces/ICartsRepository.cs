using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface ICartsRepository : IReadRepository<Cart>
{
    /// <summary>
    /// Provides the cart with the given <paramref name="cartId"/>, along with the cart products and their original product entities.
    /// </summary>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="asReadOnly">Boolean that indicates if the query should be optimized for data that will only be read.</param>
    /// <returns>
    /// A task containing the cart entity with its cart products and their original products loaded. If no instance is found the operation returns null.
    /// </returns>
    Task<Cart?> GetCartWithProductsDetailsAsync(Guid cartId, bool asReadOnly = false);
    
    /// <summary>
    /// Provides the cart of the user with the given <paramref name="userId"/>, along with the cart products and their original product entities.
    /// </summary>
    /// <param name="userId">The user to which the cart belongs.</param>
    /// <param name="asReadOnly">Boolean that indicates if the query should be optimized for data that will only be read.</param>
    /// <returns>
    /// A task containing the cart entity with its cart products and their original products loaded. If no instance is found the operation returns null.
    /// </returns>
    Task<Cart?> GetUserCartWithProductsDetailsAsync(Guid userId, bool asReadOnly = false);

    /// <summary>
    /// Provides the cart of the user with the given <paramref name="userId"/>, along with the cart products. 
    /// </summary>
    /// <param name="userId">The user to which the cart belongs.</param>
    /// <returns>
    /// A task containing the cart entity with its cart products loaded. 
    /// </returns>
    Task<Cart?> GetCartWithCartProductsByUserIdAsync(Guid userId);

    Task<bool> AddAsync(Cart cart);

    Task<bool> AddRangeAsync(IEnumerable<Cart> carts);

    Task<bool> UpdateAsync(Cart cart);

    Task<bool> DeleteAsync(Cart cart);
}
