using System.Linq.Expressions;

namespace TradeNest.Data.Repository.Interfaces;

/// <summary>
/// Interface that provides the user the options to declare specifications for the data that is requested.
/// </summary>
/// <typeparam name="T">The model that holds the requested data.</typeparam>
public interface IQueryBuilder<T> where T : class, new()
{
    IQueryBuilder<T> AddFilter(Expression<Func<T, bool>> filter);
    
    IQueryBuilder<T> WithRelated(Expression<Func<T, object>> includeStatement);
    
    IQueryBuilder<T> AddOrderAsc(Expression<Func<T, object>> orderByStatement);
    
    IQueryBuilder<T> AddOrderDesc(Expression<Func<T, object>> descendingOrderByStatement);
}