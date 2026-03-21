using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;

namespace TradeNest.Data.Repository;

public class OrdersRepository : BaseRepository<Order>, IOrdersRepository
{
    public OrdersRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public override Task<bool> AddAsync(Order entity)
    {
        try
        {
            return base.AddAsync(entity);
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            throw new DataConcurrencyConflictException(innerException: concurrencyException);
        }
    }
}