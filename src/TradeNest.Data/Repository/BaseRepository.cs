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

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Action<QueryBuilder<T>>? queryOptionsBuilder = null)
    {
        if (queryOptionsBuilder == null)
        {
            return await this.DbContext.Set<T>()
                .ToArrayAsync();
        }
        
        return await this.BuildQuery(queryOptionsBuilder)
            .ToArrayAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsReadOnlyAsync(
        Action<IQueryBuilder<T>>? queryOptionsBuilder = null)
    {
        if (queryOptionsBuilder == null)
        {
            return await this.DbContext.Set<T>()
                .ToArrayAsync();
        }
        
        return await this.BuildQuery(queryOptionsBuilder, asReadOnly: true)
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
        
        return res > 0;
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        this.DbContext.Set<T>().Update(entity);
        int res = await this.DbContext.SaveChangesAsync();
        
        return res > 0;
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
        
        return res >= entitiesAsArr.Count();
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

    protected IQueryable<T> BuildQuery(Action<QueryBuilder<T>> queryOptionsBuilder, 
        IQueryable<T>? queryable = null, bool? asReadOnly = null)
    {
        queryable ??= this.DbContext.Set<T>();
        
        QueryBuilder<T> queryBuilder = new QueryBuilder<T>();
        queryOptionsBuilder.Invoke(queryBuilder);
        
        if (queryBuilder.Filter != null)
            queryable = queryable.Where(queryBuilder.Filter);
        
        foreach (Expression<Func<T, object>> includeStatement in queryBuilder.IncludesList)
            queryable = queryable.Include(includeStatement);

        if (asReadOnly is true)
            queryable = queryable.AsNoTracking();

        foreach (Expression<Func<T, object>> orderingStatement in queryBuilder.OrderByAscendingStatements.Reverse())
            queryable = queryable.OrderBy(orderingStatement);

        foreach (Expression<Func<T, object>> descOrderingStatement in queryBuilder.OrderByDescendingStatements.Reverse())
            queryable = queryable.OrderByDescending(descOrderingStatement);

        return queryable;
    }
}