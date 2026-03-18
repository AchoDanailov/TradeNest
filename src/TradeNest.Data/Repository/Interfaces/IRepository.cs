using System.Linq.Expressions;

namespace TradeNest.Data.Repository.Interfaces;

public interface IRepository<T> : IDisposable
    where T : class, new()
{
    Task<IEnumerable<T>> GetAllAsync(Action<QueryOptions<T>>? optionsSetter = null);
    Task<IEnumerable<T>> GetAllAsReadOnlyAsync(Action<IQueryOptions<T>>? optionsSetter = null);
    
    Task<T?> FindByIdAsync(Guid id); 
    
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
    
    Task<bool> AddAsync(T entity);

    Task<bool> UpdateAsync(T entity);

    Task<bool> DeleteAsync(T entity);

    Task<bool> AddRangeAsync(IEnumerable<T> entities);

    Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> filter);
}