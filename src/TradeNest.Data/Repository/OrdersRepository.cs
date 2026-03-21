using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class OrdersRepository : BaseRepository<Order>, IOrdersRepository
{
    public OrdersRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }
}