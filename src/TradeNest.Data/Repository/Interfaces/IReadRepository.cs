using System.Linq.Expressions;
using TradeNest.Data.QueryOptions.Interfaces;

namespace TradeNest.Data.Repository.Interfaces;

// TODO: Remove the hybrid repository (IReadRepository<T>) and move to per entity repository.
public interface IReadRepository<T> : IDisposable
    where T : class, new()
{
    Task<IEnumerable<T>> GetAllAsync(Action<IQueryOptions<T>>? queryOptionsBuilder = null);
    
    Task<T?> FindByIdAsync(Guid id); 
    
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
}