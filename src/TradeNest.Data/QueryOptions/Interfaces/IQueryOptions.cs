using System.Linq.Expressions;

namespace TradeNest.Data.QueryOptions.Interfaces;

/// <summary>
/// Interface that provides options to declare specifications about the data that is requested.
/// </summary>
/// <typeparam name="T">The model type of the requested data.</typeparam>
public interface IQueryOptions<T> where T : class, new()
{
    bool IsReadonly { get; }
    int? Page { get; }
    int? Limit { get; }
    Expression<Func<T, bool>>? Filter { get; }
    IReadOnlyCollection<Expression<Func<T, object>>> IncludesList { get; }
    IReadOnlyCollection<(Expression<Func<T, object>>, bool)> OrderExpressionsByDirection { get; }
    
    IQueryOptions<T> AsReadOnly();   
    
    IQueryOptions<T> SetFilter(Expression<Func<T, bool>> filter);
    
    IQueryOptions<T> WithRelated(Expression<Func<T, object>> includeStatement);
    
    IQueryOptions<T> AddOrderAsc(Expression<Func<T, object>> orderByStatement);
    
    IQueryOptions<T> AddOrderDesc(Expression<Func<T, object>> descendingOrderByStatement);

    IQueryOptions<T> WithPagination(int page, int? limit);
}