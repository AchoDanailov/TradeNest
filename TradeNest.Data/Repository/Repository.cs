using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class Repository<T> : IRepository<T> 
    where T : class
{
    private readonly TradeNestDbContext _dbContext;

    public Repository(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }
    
    public IQueryable<T> AllAsReadonlyAsync()
    {
        return this._dbContext.Set<T>().AsNoTracking();
    }

    public IQueryable<T> AllAsync()
    {
        return this._dbContext.Set<T>();
    }

    public async Task<T?> FindByIdAsync(Guid id)
    {
        return await this._dbContext.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await this._dbContext.Set<T>().AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> items)
    {
        await this._dbContext.Set<T>().AddRangeAsync(items);
    }

    public void Remove(T item)
    {
        this._dbContext.Set<T>().Remove(item);
    }

    public async Task<int> ExecuteRemoveRangeAsync(Expression<Func<T, bool>> filter)
    {
        return await this._dbContext.Set<T>()
            .Where(filter)
            .ExecuteDeleteAsync();
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await this._dbContext.SaveChangesAsync();
    }
}