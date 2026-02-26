using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class Repository : IRepository 
{
    private readonly TradeNestDbContext _dbContext;

    public Repository(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }
    
    public IQueryable<T> AllAsReadonly<T>() where T : class
    {
        return this._dbContext.Set<T>().AsNoTracking();
    }

    public IQueryable<T> All<T>() where T : class
    {
        return this._dbContext.Set<T>();
    }

    public async Task<T?> FindByIdAsync<T>(Guid id) where T : class
    {
        return await this._dbContext.Set<T>().FindAsync(id);
    }

    public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> filter) where T : class
    {
        return await this._dbContext.Set<T>().AnyAsync(filter);
    }

    public async Task AddAsync<T>(T entity) where T : class
    {
        await this._dbContext.Set<T>().AddAsync(entity);
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> items) where T : class
    {
        await this._dbContext.Set<T>().AddRangeAsync(items);
    }

    public void Remove<T>(T item) where T : class
    {
        this._dbContext.Set<T>().Remove(item);
    }

    public async Task<int> ExecuteRemoveRangeAsync<T>(Expression<Func<T, bool>> filter) where T : class
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