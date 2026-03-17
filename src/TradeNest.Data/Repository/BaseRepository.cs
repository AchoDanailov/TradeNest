using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public abstract class BaseRepository<T> : IRepository<T>
    where T : class, new()
{
    private readonly TradeNestDbContext _dbContext;
    private bool _disposed = false;

    protected BaseRepository(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    protected TradeNestDbContext DbContext => this._dbContext;
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await this.DbContext.Set<T>()
            .ToArrayAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsReadOnlyAsync()
    {
        return await this.DbContext.Set<T>()
            .AsNoTracking()
            .ToArrayAsync();
    }

    public virtual async Task<T?> FindByIdAsync(Guid id)
    {
        return await this.DbContext.Set<T>()
            .FindAsync(id);
    }
    
    public virtual async Task<bool> AddAsync(T entity)
    {
        await this.DbContext.Set<T>().AddAsync(entity);
        int res = await this.DbContext.SaveChangesAsync();
        return res == 1;
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        this.DbContext.Set<T>().Update(entity);
        int res = await this.DbContext.SaveChangesAsync();
        return res == 1;
    }

    public virtual async Task<bool> DeleteAsync(T entity)
    {
        this.DbContext.Set<T>().Remove(entity);
        int res = await this.DbContext.SaveChangesAsync();
        return res == 1;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        return await this.DbContext.Set<T>()
            .AnyAsync(filter);
    }

    public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities)
    {
        IEnumerable<T> entitiesAsArr = entities.ToArray();
        await this.DbContext.Set<T>().AddRangeAsync(entitiesAsArr);
        int res = await this.DbContext.SaveChangesAsync();
        return res == entitiesAsArr.Count();
    }

    public virtual async Task<bool> ExecuteDelete(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> targetEntries = this.DbContext.Set<T>().Where(filter);
        await targetEntries.ExecuteDeleteAsync();
        int res = await this.DbContext.SaveChangesAsync();
        return res == targetEntries.Count();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._dbContext.Dispose();
            }
        }
        this._disposed = true;
    }
}