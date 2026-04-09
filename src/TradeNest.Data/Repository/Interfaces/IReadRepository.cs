using System.Linq.Expressions;

namespace TradeNest.Data.Repository.Interfaces;

public interface IReadRepository<T> : IDisposable
    where T : class, new()
{
    Task<IEnumerable<T>> GetAllAsync(Action<IQueryOptions<T>>? queryOptionsBuilder = null);
    
    Task<T?> FindByIdAsync(Guid id); 
    
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
}