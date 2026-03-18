using System.Linq.Expressions;

namespace TradeNest.Data.Repository.Interfaces;

public interface IQueryOptions<T> where T : class, new()
{
    IQueryOptions<T> AddProjection(Expression<Func<T, T>> projectionStatement);
    
    IQueryOptions<T> AddFilter(Expression<Func<T, bool>> filter);
    
    IQueryOptions<T> Include(Expression<Func<T, object>> includeStatement);
    
    IQueryOptions<T> OrderBy(Expression<Func<T, object>> orderByStatement);
    
    IQueryOptions<T> OrderByDescending(Expression<Func<T, object>> descendingOrderByStatement);
}