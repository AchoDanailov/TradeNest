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
    Task<IEnumerable<AllOrdersViewModel>> GetAllOrdersByUserIdAsync(Guid userId);
}