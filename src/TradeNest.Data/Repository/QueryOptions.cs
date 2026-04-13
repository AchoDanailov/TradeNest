using System.Linq.Expressions;

using static TradeNest.GCommon.ApplicationConstants;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class QueryOptions<T> : IQueryOptions<T>
    where T : class, new()
{
    private readonly List<Expression<Func<T, object>>> _includesList;
    private readonly List<(Expression<Func<T, object>>, bool)> _orderStatementsByDirection;
    
    private bool _isReadonly;
    private int? _page;
    private int? _limit;
    
    internal QueryOptions()
    {
        this._includesList = new List<Expression<Func<T, object>>>();
        this._orderStatementsByDirection = new List<(Expression<Func<T, object>>, bool)>();
    }

    internal bool IsReadonly => this._isReadonly;
    internal int? Page => this._page;
    internal int? Limit => this._limit;
    
    internal Expression<Func<T, bool>>? Filter { get; private set; }

    internal IReadOnlyCollection<Expression<Func<T, object>>> IncludesList
        => this._includesList.AsReadOnly();

    internal IReadOnlyCollection<(Expression<Func<T, object>>, bool)> OrderExpressionsByDirection 
        => this._orderStatementsByDirection.AsReadOnly();


    public IQueryOptions<T> AsReadOnly()
    {
        this._isReadonly = true;
        return this;
    }

    public IQueryOptions<T> AddFilter(Expression<Func<T, bool>> filter)
    {
        this.Filter = filter;
        return this;
    }

    public IQueryOptions<T> WithRelated(Expression<Func<T, object>> includeStatement)
    {
        this._includesList.Add(includeStatement);
        return this;
    }

    public IQueryOptions<T> AddOrderAsc(Expression<Func<T, object>> orderByStatement)
    {
        this._orderStatementsByDirection
            .Add(new ValueTuple<Expression<Func<T, object>>, bool>(orderByStatement, true));
        return this;
    }
    
    public IQueryOptions<T> AddOrderDesc(Expression<Func<T, object>> descendingOrderByStatement)
    {
        this._orderStatementsByDirection
            .Add(new ValueTuple<Expression<Func<T, object>>, bool>(descendingOrderByStatement, false));
        return this;
    }

    public IQueryOptions<T> WithPagination(int page, int? limit = DefaultPaginationLimitValue)
    {
        this._page = page;
        this._limit = limit;
        return this;
    }
}