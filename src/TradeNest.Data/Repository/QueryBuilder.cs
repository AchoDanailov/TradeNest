using System.Linq.Expressions;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class QueryBuilder<T> : IQueryBuilder<T>
    where T : class, new()
{
    private readonly List<Expression<Func<T, object>>> _includesList;
    private readonly List<Expression<Func<T, object>>> _orderByAscendingStatements;
    private readonly List<Expression<Func<T, object>>> _orderByDescendingStatements;
    
    internal QueryBuilder()
    {
        this._includesList = new List<Expression<Func<T, object>>>();
        this._orderByAscendingStatements = new List<Expression<Func<T, object>>>();
        this._orderByDescendingStatements = new List<Expression<Func<T, object>>>();
    }
    
    internal Expression<Func<T, bool>>? Filter { get; private set; }

    internal IReadOnlyCollection<Expression<Func<T, object>>> IncludesList
        => this._includesList.AsReadOnly();

    internal IReadOnlyCollection<Expression<Func<T, object>>> OrderByAscendingStatements
        => this._orderByAscendingStatements.AsReadOnly();

    internal IReadOnlyCollection<Expression<Func<T, object>>> OrderByDescendingStatements
        => this._orderByDescendingStatements.AsReadOnly();


    public IQueryBuilder<T> AddFilter(Expression<Func<T, bool>> filter)
    {
        this.Filter = filter;
        return this;
    }

    public IQueryBuilder<T> WithRelated(Expression<Func<T, object>> includeStatement)
    {
        this._includesList.Add(includeStatement);
        return this;
    }

    public IQueryBuilder<T> AddOrderAsc(Expression<Func<T, object>> orderByStatement)
    {
        this._orderByAscendingStatements.Add(orderByStatement);
        return this;
    }
    
    public IQueryBuilder<T> AddOrderDesc(Expression<Func<T, object>> descendingOrderByStatement)
    {
        this._orderByDescendingStatements.Add(descendingOrderByStatement);
        return this;
    }
}