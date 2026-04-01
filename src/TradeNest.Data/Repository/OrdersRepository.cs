using Microsoft.EntityFrameworkCore;

using TradeNest.GCommon.Exceptions;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class OrdersRepository : BaseReadRepository<Order>, IOrdersRepository
{
    public OrdersRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<bool> AddAsync(Order order)
    {
        try
        {
            Cart userCartToBeDeleted = this.DbContext.Carts
                .Single(c => c.CartOwnerId == order.UserId);
            this.DbContext.Remove(userCartToBeDeleted);

            await this.DbContext.Orders.AddAsync(order);
            int res = await this.DbContext.SaveChangesAsync();

            return res >= 1;
        }
        catch (DbUpdateConcurrencyException concurrencyEx)
        {
            this.DbContext.ChangeTracker.Clear();
            throw new DataConcurrencyConflictException(innerException: concurrencyEx);
        }
    }
}