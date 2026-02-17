using TradeNest.Web.ViewModels.Order;

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
    Task<IEnumerable<OrderViewModel>> GetAllOrdersByUserIdAsync(Guid userId);

    /// <summary>
    /// Adds the provided product to the user's ongoing order.
    /// If no order is currently ongoing - a new ongoing order is created.
    /// </summary>
    /// <param name="userId">The user identifier which is attempting the operation.</param>
    /// <param name="productId">The product identifier which is added to the user's ongoing order.</param>
    /// <param name="prodQtyToAdd">The quantity of the product being added to the ongoing order.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    Task AddProductToOrderAsync(Guid userId, Guid productId, int prodQtyToAdd);

    /// <summary>
    /// Gets the user's ongoing order if any.
    /// </summary>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <returns>
    /// A Task that contains the user's order ongoing order if any.
    /// If user's ongoing order is not found - the method returns null.
    /// </returns>
    Task<OrderViewModel?> GetUserOngoingOrderWithProductsAsync(Guid userId);

    /// <summary>
    /// Removes the product from the user's ongoing order.
    /// </summary>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <param name="productId">
    /// The product identifier which is being removed from the user's ongoing order.
    /// </param>
    /// <param name="orderId">The user's ongoing order identifier.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
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
    Task CancelOrderAsync(Guid userId, Guid orderId);

    /// <summary>
    /// Submits the user's ongoing order.
    /// </summary>
    /// <param name="orderId">The order to be submitted</param>
    /// <param name="userId">The user identifier to which the orders belong.</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    Task SubmitOrderAsync(Guid userId, Guid orderId);
}