using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class OrdersRepository : BaseRepository<Order>, IOrdersRepository
{
    public OrdersRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public override async Task<bool> AddAsync(Order entity)
    {
        Cart userCartToBeDeleted = this.DbContext.Carts
            .Single(c => c.CartOwnerId == entity.UserId);
        this.DbContext.Remove(userCartToBeDeleted);
        
        return await base.AddAsync(entity);
    }
}