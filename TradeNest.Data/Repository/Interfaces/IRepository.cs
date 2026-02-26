using System.Linq.Expressions;

namespace TradeNest.Data.Repository.Interfaces;

public interface IRepository
{
    IQueryable<T> AllAsReadonly<T>() where T : class;

    IQueryable<T> All<T>() where T : class;

    Task<T?> FindByIdAsync<T>(Guid id) where T : class;

    Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> filter) where T : class;

    Task AddAsync<T>(T item) where T : class;

    Task AddRangeAsync<T>(IEnumerable<T> items) where T : class;

    void Remove<T>(T item) where T : class;

    Task<int> ExecuteRemoveRangeAsync<T>(Expression<Func<T, bool>> filter) where T : class;
    
    Task<int> SaveChangesAsync();
}