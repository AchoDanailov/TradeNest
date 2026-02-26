using System.Linq.Expressions;

namespace TradeNest.Data.Repository.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> AllAsReadonlyAsync();

    IQueryable<T> AllAsync();

    Task<T?> FindByIdAsync(Guid id);

    Task AddAsync(T item);

    Task AddRangeAsync(IEnumerable<T> items);

    void Remove(T item);

    Task<int> ExecuteRemoveRangeAsync(Expression<Func<T, bool>> filter);
    
    Task<int> SaveChangesAsync();
}