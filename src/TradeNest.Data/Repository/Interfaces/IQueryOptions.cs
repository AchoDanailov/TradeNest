using System.Linq.Expressions;

namespace TradeNest.Data.Repository.Interfaces;

/// <summary>
/// Interface that provides options to declare specifications about the data that is requested.
/// </summary>
/// <typeparam name="T">The model type of the requested data.</typeparam>
public interface IQueryOptions<T> where T : class, new()
{
    IQueryOptions<T> AsReadOnly();   
    
    IQueryOptions<T> AddFilter(Expression<Func<T, bool>> filter);
    
    IQueryOptions<T> WithRelated(Expression<Func<T, object>> includeStatement);
    
    IQueryOptions<T> AddOrderAsc(Expression<Func<T, object>> orderByStatement);
    
    IQueryOptions<T> AddOrderDesc(Expression<Func<T, object>> descendingOrderByStatement);

    IQueryOptions<T> WithPagination(int page, int? limit);
}