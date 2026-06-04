using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data.QueryOptions;

public static class QueryOptionsTranslator<T> 
    where T : class, new()
{
    public static IQueryable<T> ToQueryable(IQueryable<T> queryable,
        Action<QueryOptions<T>> queryOptionsBuilder)
    {
        QueryOptions<T> queryOptions = new QueryOptions<T>();
        queryOptionsBuilder.Invoke(queryOptions);
        
        if (queryOptions.Filter != null)
            queryable = queryable.Where(queryOptions.Filter);
        
        foreach (Expression<Func<T, object>> includeStatement in queryOptions.IncludesList)
            queryable = queryable.Include(includeStatement);

        if (queryOptions.IsReadonly)
            queryable = queryable.AsNoTracking();

        if (queryOptions.OrderExpressionsByDirection.Any())
        {
            IEnumerable<(Expression<Func<T, object>>, bool)> orderExprsByDir
                = queryOptions.OrderExpressionsByDirection.ToList();
            
            (Expression<Func<T, object>> firstExpr, bool isAscDirection) = orderExprsByDir.First();
            IOrderedQueryable<T> orderedQueryable = isAscDirection
                ? queryable.OrderBy(firstExpr)
                : queryable.OrderByDescending(firstExpr);

            foreach ((Expression<Func<T, object>> expr, bool isAsc) orderExprByDir in orderExprsByDir.Skip(1))
            {
                orderedQueryable = orderExprByDir.isAsc
                    ? orderedQueryable.ThenBy(orderExprByDir.expr)
                    : orderedQueryable.ThenByDescending(orderExprByDir.expr);
            }

            queryable = orderedQueryable;
        }

        if (queryOptions.Page != null && queryOptions.Limit != null)
        {
            int skipCount = (queryOptions.Page.Value - 1) * queryOptions.Limit.Value;
            queryable = queryable
                .Skip(skipCount)
                .Take(queryOptions.Limit.Value);
        }

        return queryable;
    }
}