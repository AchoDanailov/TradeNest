using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IOrdersRepository : IReadRepository<Order>
{
    Task<bool> AddAsync(Order order);
}