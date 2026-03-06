using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models;

namespace TradeNest.Services.Core.Interfaces;

public interface IOrdersService
{
    /// <summary>
    /// Provides a collection with all user orders.
    /// </summary>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <returns>
    /// Collection with the user's orders.
    /// If none are found the method returns an empty collection.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided <paramref name="userId"/> is with value <see cref="Guid.Empty"/>,
    /// or user with <paramref name="userId"/> does not exist.
    /// </exception>
    Task<IEnumerable<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId);

    /// <summary>
    /// Adds the provided product to the user's ongoing order.
    /// If no order is currently ongoing - a new ongoing order is created.
    /// </summary>
    /// <param name="userId">The user identifier which is attempting the operation.</param>
    /// <param name="productId">The product identifier which is added to the user's ongoing order.</param>
    /// <param name="prodQtyToAdd">The quantity of the product being added to the ongoing order.</param>
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
    /// Thrown if there is less quantity in stock of the given product than the user attempts
    /// to add to his order.
    /// </exception>
    /// <exception cref="OrderingDisabledProductException">
    /// Thrown if the product with <paramref name="productId"/> has status disabled.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user with <paramref name="userId"/> is the owner of the product with the
    /// provided <paramref name="productId"/>
    /// </exception>
    Task AddProductToOrderAsync(Guid userId, Guid productId, int prodQtyToAdd);

    /// <summary>
    /// Gets the user's ongoing order if any.
    /// </summary>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <returns>
    /// A Task that contains the user's order ongoing order if any.
    /// If user's ongoing order is not found - the method returns null.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="userId"/> is with value <see cref="Guid.Empty"/> or
    /// user with <paramref name="userId"/> does not exist.
    /// </exception>
    Task<OrderDto?> GetUserOngoingOrderWithProductsAsync(Guid userId);

    /// <summary>
    /// Removes the product from the user's ongoing order.
    /// </summary>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <param name="productId">
    /// The product identifier which is being removed from the user's ongoing order.
    /// </param>
    /// <param name="orderId">The user's ongoing order identifier.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId"/>, <paramref name="productId"/> or
    /// <paramref name="orderId"/> are with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if user with <paramref name="userId"/> does not exist.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if a product with <paramref name="productId"/> does not exist,
    /// or if order with <paramref name="orderId"/> does not exist.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the order with <paramref name="orderId"/> is not that of a user with
    /// <paramref name="userId"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user with <paramref name="userId"/> is the owner of the product
    /// with <paramref name="productId"/>.
    /// Thrown if the order with <paramref name="orderId"/> is already submitted.
    /// Thrown if the product with <paramref name="productId"/> is not listed in the order
    /// with <paramref name="orderId"/>.
    /// </exception>
    Task RemoveProductFromOrderAsync(Guid userId, Guid productId, Guid orderId);

    /// <summary>
    /// Determines whether an order with the specified identifier exists.
    /// </summary>
    /// <param name="orderId">The order's identifier.</param>
    /// <returns>A task that returns true if the order exists; otherwise, false.</returns>
    Task<bool> OrderExistsByIdAsync(Guid orderId);

    /// <summary>
    /// Cancels the user's ongoing order.
    /// </summary>
    /// <param name="orderId">The order's identifier</param>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if either <paramref name="userId"/> or <paramref name="orderId"/>
    /// is of value <see cref="Guid.Empty"/>
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if order with <paramref name="orderId"/> does not exist.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the order with <paramref name="orderId"/> is not that of a user with
    /// <paramref name="userId"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the order with <paramref name="orderId"/> is already submitted.
    /// </exception>
    Task CancelOrderAsync(Guid userId, Guid orderId);

    /// <summary>
    /// Submits the user's ongoing order.
    /// </summary>
    /// <param name="orderId">The order to be submitted</param>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if either <paramref name="userId"/> or <paramref name="orderId"/> are with
    /// value <see cref="Guid.Empty"/>. Also thrown if the user with <paramref name="userId"/>
    /// does not exist.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if the order with the provided <paramref name="orderId"/> does not exist.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the order with <paramref name="orderId"/> is not that of a user with
    /// <paramref name="userId"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the order with <paramref name="orderId"/> is already submitted.
    /// </exception>
    /// <exception cref="InsufficientProductQuantityInStockException">
    /// Thrown if any of the products in the order with <paramref name="orderId"/>
    /// does not satisfy the quantity requested to submit. 
    /// </exception>
    /// <exception cref="OrderingDisabledProductException">
    /// Thrown if the order contains a product that has changed status to "Disabled".
    /// </exception>
    /// <remarks>Keep and mind that if the service throws
    /// <see cref="InsufficientProductQuantityInStockException"/> or
    /// <see cref="OrderingDisabledProductException"/>, the targeted product will be automatically
    /// removed from the ongoing order.
    /// </remarks>
    Task SubmitOrderAsync(Guid userId, Guid orderId);

    /// <summary>
    /// Validates that the given user can add to his order the given quantity of
    /// specified product.
    /// </summary>
    /// <param name="userId">The identifier of the user attempting to add product to his order.</param>
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
    Task<bool> IsValidProductQtyToOrderAsync(Guid userId, ProductQtyValidationDto model);
}