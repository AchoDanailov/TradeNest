using System.Linq.Expressions;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class QueryBuilder<T> : IQueryBuilder<T>
    where T : class, new()
{
    private readonly List<Expression<Func<T, object>>> _includesList;
    private readonly List<(Expression<Func<T, object>>, bool)> _orderStatementsByDirection;
    
    internal QueryBuilder()
    {
        this._includesList = new List<Expression<Func<T, object>>>();
        this._orderStatementsByDirection = new List<(Expression<Func<T, object>>, bool)>();
    }
    
    internal Expression<Func<T, bool>>? Filter { get; private set; }

    internal IReadOnlyCollection<Expression<Func<T, object>>> IncludesList
        => this._includesList.AsReadOnly();

    internal IReadOnlyCollection<(Expression<Func<T, object>>, bool)> OrderExpressionsByDirection 
        => this._orderStatementsByDirection.AsReadOnly();


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
        this._orderStatementsByDirection.Add((orderByStatement, true));
        return this;
    }
    
    public IQueryBuilder<T> AddOrderDesc(Expression<Func<T, object>> descendingOrderByStatement)
    {
        this._orderStatementsByDirection.Add((descendingOrderByStatement, false));
        return this;
    }
}