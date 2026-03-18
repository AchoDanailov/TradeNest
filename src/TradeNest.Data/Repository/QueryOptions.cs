using System.Linq.Expressions;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class QueryOptions<T> : IQueryOptions<T>
    where T : class, new() 
{
    internal QueryOptions()
    {
        this.IncludeList = new List<Expression<Func<T, object>>>().AsReadOnly();
        this.OrderByAscendingStatements = new List<Expression<Func<T, object>>>().AsReadOnly();
        this.OrderByDescendingStatements = new List<Expression<Func<T, object>>>().AsReadOnly();
    }
    
    internal Expression<Func<T, T>>? Projection { get; private set; }
    
    internal Expression<Func<T, bool>>? Filter { get; private set; }

    internal ICollection<Expression<Func<T, object>>> IncludeList { get; }
        
    internal ICollection<Expression<Func<T, object>>> OrderByAscendingStatements { get; }
    
    internal ICollection<Expression<Func<T, object>>> OrderByDescendingStatements { get; }

    public IQueryOptions<T> AddProjection(Expression<Func<T, T>> projectionStatement)
    {
        this.Projection = projectionStatement;
        return this;
    }
    
    public IQueryOptions<T> AddFilter(Expression<Func<T, bool>> filter)
    {
        this.Filter = filter;
        return this;
    }

    public IQueryOptions<T> Include(Expression<Func<T, object>> includeStatement)
    {
        this.IncludeList.Add(includeStatement);
        return this;
    }

    public IQueryOptions<T> OrderBy(Expression<Func<T, object>> orderByStatement)
    {
        this.OrderByAscendingStatements.Add(orderByStatement);
        return this;
    }
    
    public IQueryOptions<T> OrderByDescending(Expression<Func<T, object>> descendingOrderByStatement)
    {
        this.OrderByDescendingStatements.Add(descendingOrderByStatement);
        return this;
    }
}