using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using TradeNest.Data.QueryOptions;
using TradeNest.Data.QueryOptions.Interfaces;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public abstract class BaseReadRepository<T> : IReadRepository<T>
    where T : class, new()
{
    private readonly TradeNestDbContext _dbContext;
    private bool _disposed = false;

    protected BaseReadRepository(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    protected TradeNestDbContext DbContext => this._dbContext;

    public async Task<IEnumerable<T>> GetAllAsync(
        Action<IQueryOptions<T>>? queryOptionsBuilder = null)
    {
        if (queryOptionsBuilder == null)
        {
            return await this.DbContext.Set<T>()
                .ToArrayAsync();
        }
        
        return await QueryOptionsTranslator<T>
            .ToQueryable(this.DbContext.Set<T>(), queryOptionsBuilder)
            .ToArrayAsync();
    }

    public async Task<T?> FindByIdAsync(Guid id)
    {
        return await this.DbContext.Set<T>()
            .FindAsync(id);
    }
    
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        return await this.DbContext.Set<T>()
            .AnyAsync(filter);
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