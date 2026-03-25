using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models.Cart;
using TradeNest.Services.Models.Product;

namespace TradeNest.Services.Core.Interfaces;

public interface ICartsService
{
    /// <summary>
    /// Provides the user's cart.
    /// </summary>
    /// <param name="userId">The cart's owner identifier.</param>
    /// <returns>
    /// Task that holds the user's Cart. Returns null if user has no ongoing cart.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if a user with the provided <paramref name="userId" /> does not exist.
    /// </exception>
    Task<CartDto?> GetCartByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Adds the provided product to the user's cart.
    /// If user currently has no cart. A new one is created.
    /// </summary>
    /// <param name="userId">The user identifier which is attempting the operation.</param>
    /// <param name="productId">The product identifier which is added to the user's cart.</param>
    /// <param name="prodQtyToAdd">The quantity of the product being added to the cart.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if either <paramref name="userId"/>, <paramref name="productId"/> are with value
    /// <see cref="Guid.Empty"/>, or if <paramref name="prodQtyToAdd"/> is with value zero or
    /// negative number.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if user with <paramref name="userId"/> does not exist.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if a product with the provided <paramref name="productId"/> does not exist.
    /// </exception>
    /// <exception cref="InsufficientProductQuantityInStockException">
    /// Thrown if there is less quantity in stock of the given product than requested
    /// product quantity to add (<paramref name="prodQtyToAdd" />).
    /// </exception>
    /// <exception cref="ProductDisabledException">
    /// Thrown if the product with <paramref name="productId"/> has status disabled.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user with <paramref name="userId"/> is the owner of the product with the
    /// provided <paramref name="productId"/>
    /// </exception>
    /// <exception cref="DataPersistException">
    /// Thrown if the data is not successfully persisted.
    /// </exception>
    Task AddProductToCartAsync(Guid userId, Guid productId, int prodQtyToAdd);
    
    /// <summary>
    /// Removes the product from the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <param name="productId">
    /// The product identifier which is being removed from the user's ongoing order.
    /// </param>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId"/>, <paramref name="productId"/>
    /// are with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if user with <paramref name="userId"/> does not exist.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if user has no products in his cart or product with <paramref name="productId"/>
    /// is not found in the cart.
    /// Thrown if the product with <paramref name="productId"/> is not listed in the cart.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if a product with <paramref name="productId"/> does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user with <paramref name="userId"/> is the owner of the product
    /// with <paramref name="productId"/>.
    /// </exception>
    /// <exception cref="DataPersistException">
    /// Thrown if the data is not successfully persisted.
    /// </exception>
    Task RemoveProductFromCartAsync(Guid userId, Guid productId);
    
    /// <summary>
    /// Cancels the user's order / removes the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="userId"/> is of value <see cref="Guid.Empty"/> or if
    /// the user with <paramref name="userId"/> does not exist or does not have a cart.
    /// </exception>
    /// <exception cref="DataPersistException">
    /// Thrown if the data is not successfully persisted.
    /// </exception>
    Task DeleteCart(Guid userId);

    /// <summary>
    /// Validates that the given user can add to his cart the given quantity of
    /// specified product.
    /// </summary>
    /// <param name="userId">The identifier of the user attempting to add product to his cart.</param>
    /// <param name="model">The model that holds data about the product and the quantity.</param>
    /// <returns>
    /// Task containing a value that represents if the user can add the given product quantity to his order.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the user with <paramref name="userId"/> or the product with
    /// <paramref name="model.Id"/> are with value <see cref="Guid.Empty"/>, or if user
    /// with <paramref name="userId"/> does not exist.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if product with the provided id in <paramref name="model"/> does not exist.
    /// </exception>
    Task<bool> IsValidProductQtyToAddToCartAsync(Guid userId, ProductQtyValidationDto model);

    /// <summary>
    /// Updates the cart product in the user cart with the provided values in the
    /// <see cref="UpdateCartProductDto"/>.
    /// </summary>
    /// <param name="userId">The user identifier of the user attempting the operation.</param>
    /// <param name="updateCartProductDto">
    /// The object that contains the values that can be updated.
    /// </param>
    /// <returns>
    /// Task that contains a bool representing weather the operation was successful or not.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId"/> or
    /// <paramref name="updateCartProductDto.ProductId"/> or
    /// <paramref name="updateCartProductDto.CartId"/> are with value <see cref="Guid.Empty"/>
    /// or if user with the provided identifier does not exist.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if a cart with <paramref name="updateCartProductDto.CartId"/> and
    /// is not found or if a cart product with the provided
    /// <paramref name="updateCartProductDto.ProductId"/> is not found in the user cart.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the user with <paramref name="userId"/> attempting the operation is not
    /// the owner of the cart.
    /// </exception>
    /// <exception cref="InsufficientProductQuantityInStockException">
    /// Thrown if the quantity requested from the
    /// <paramref name="updateCartProductDto.Quantity"/> is more than the
    /// product quantity in stock.
    /// </exception>
    Task<bool> UpdateCartProduct(Guid userId, UpdateCartProductDto updateCartProductDto);
}