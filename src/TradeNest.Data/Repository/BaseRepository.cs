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

    public abstract Task<bool> DeleteAsync(T entity);

    public abstract Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> filter);

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Action<QueryOptions<T>>? optionsSetter = null)
    {
        if (optionsSetter == null)
        {
            return await this.DbContext.Set<T>()
                .ToArrayAsync();
        }
        
        return await this.BuildQuery(optionsSetter)
            .ToArrayAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsReadOnlyAsync(
        Action<IQueryOptions<T>>? optionsSetter = null)
    {
        if (optionsSetter == null)
        {
            return await this.DbContext.Set<T>()
                .ToArrayAsync();
        }
        
        return await this.BuildQuery(optionsSetter, isReadOnly: true)
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

    protected  IQueryable<T> BuildQuery(Action<QueryOptions<T>> optionsSetter, bool? isReadOnly = null)
    {
        IQueryable<T> queryable = this.DbContext.Set<T>();
        
        QueryOptions<T> queryOptions = new QueryOptions<T>();
        optionsSetter.Invoke(queryOptions);
        
        if (queryOptions.Filter != null)
            queryable = queryable.Where(queryOptions.Filter);
        
        if (queryOptions.Projection != null)
            queryable = queryable.Select(queryOptions.Projection);
        
        foreach (Expression<Func<T, object>> includeStatement in queryOptions.IncludeList)
            queryable = queryable.Include(includeStatement);

        if (isReadOnly is true)
            queryable = queryable.AsNoTracking();

        foreach (Expression<Func<T, object>> orderingStatement in queryOptions.OrderByAscendingStatements)
            queryable = queryable.OrderBy(orderingStatement);

        foreach (Expression<Func<T, object>> descOrderingStatement in queryOptions.OrderByDescendingStatements)
            queryable = queryable.OrderByDescending(descOrderingStatement);

        
        return queryable;
    }
}