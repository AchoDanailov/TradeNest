using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models.Order;

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
    /// Determines whether an order with the specified identifier exists.
    /// </summary>
    /// <param name="orderId">The order's identifier.</param>
    /// <returns>A task that returns true if the order exists; otherwise, false.</returns>
    Task<bool> OrderExistsByIdAsync(Guid orderId);

    /// <summary>
    /// Makes and submits an order of the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier to which the cart belong.</param>
    /// <returns>A Task that holds a <see cref="SubmitOrderResultDto"/></returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="userId"/> with value <see cref="Guid.Empty"/>.
    /// Also thrown if the user with <paramref name="userId"/> does not exist.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if cart of user with the provided <paramref name="userId"/> has no products.
    /// </exception>
    Task<SubmitOrderResultDto> SubmitOrderAsync(Guid userId);
}