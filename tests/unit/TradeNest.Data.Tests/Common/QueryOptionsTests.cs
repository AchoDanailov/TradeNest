using System.Linq.Expressions;

using TradeNest.Data.Models;
using TradeNest.Data.QueryOptions;
using TradeNest.Data.QueryOptions.Interfaces;

namespace TradeNest.Data.Tests.Common;

/* Unit Tests for <see cref="QueryOptions"/>.
Keep In mind that QueryOptions is essentially just a DTO for expressions and
other values that specify business rules to how the data is fetched (similar to Specifications when using the Specification pattern).
Thus, we can test it by verifying that the methods that should store the expressions and values
in some properties actually do store those values, so that later they can be passed to the
QueryOptionsTranslator. (The tests for how queries get translated to
<see cref="Queryable"/> are left for the Integration Tests) */
[TestFixture]
public class QueryOptionsTests
{
    [Test]
    public void SetFilter_StoresExpressionsCorrectly()
    {
        // Arrange
        Expression<Func<Category, bool>> actual = (c) => c.Name.ToLower().Contains("books");
        IQueryOptions<Category> queryOptions = new QueryOptions<Category>();
        
        // Act
        queryOptions.SetFilter(actual);
        
        // Assert
        Assert.That(queryOptions.Filter, Is.EqualTo(actual));
    }

    [Test]
    public void SetFilter_OverwritesFilters_WhenFilterIsAlreadySet()
    {
        // Arrange
        Expression<Func<Category, bool>> firstFilter = (c) => c.Name.ToLower().Contains("books");
        Expression<Func<Category, bool>> secondFilter = (c) => c.Name.ToLower().Contains("electronics");
        IQueryOptions<Category> queryOptions = new QueryOptions<Category>();
        
        queryOptions.SetFilter(firstFilter);
        
        Assert.That(queryOptions.Filter, Is.EqualTo(firstFilter));

        // Act
        queryOptions.SetFilter(secondFilter);
        
        // Assert
        Assert.That(queryOptions.Filter, Is.EqualTo(secondFilter));
    }

    [Test]
    public void WithRelated_StoresIncludeExpressionsCorrectly()
    {
        // Arrange
        Expression<Func<Product, object>> include = (p) => p.Category;
        IQueryOptions<Product> queryOptions = new QueryOptions<Product>();

        // Act
        queryOptions.WithRelated(include);

        // Assert
        Assert.That(queryOptions.IncludesList, Has.Count.EqualTo(1));
        Assert.That(queryOptions.IncludesList.First(), Is.EqualTo(include));
    }

    [Test]
    public void WithRelated_StoresMultipleIncludeExpressionsCorrectly()
    {
        // Arrange
        Expression<Func<Product, object>> firstInclude = (p) => p.Category;
        Expression<Func<Product, object>> secondInclude = (p) => p.Images;
        IQueryOptions<Product> queryOptions = new QueryOptions<Product>();

        // Act
        queryOptions.WithRelated(firstInclude);
        queryOptions.WithRelated(secondInclude);

        // Assert
        Assert.That(queryOptions.IncludesList, Has.Count.EqualTo(2));
        Assert.That(queryOptions.IncludesList, Contains.Item(firstInclude));
        Assert.That(queryOptions.IncludesList, Contains.Item(secondInclude));
    }

    [Test]
    public void AddOrderAsc_StoresOrderAscExpressionCorrectly()
    {
        // Arrange
        Expression<Func<Product, object>> orderBy = (p) => p.Name;
        IQueryOptions<Product> queryOptions = new QueryOptions<Product>();

        // Act
        queryOptions.AddOrderAsc(orderBy);

        // Assert
        (Expression<Func<Product, object>>, bool) orderSpecification 
            = queryOptions.OrderExpressionsByDirection.First();
        
        Assert.That(queryOptions.OrderExpressionsByDirection, Has.Count.EqualTo(1));
        Assert.That(orderSpecification.Item1, Is.EqualTo(orderBy));
        Assert.That(orderSpecification.Item2, Is.True); // True is for asc order
    }

    [Test]
    public void AddOrderDesc_StoresOrderDescExpressionCorrectly()
    {
        // Arrange
        Expression<Func<Product, object>> orderBy = (p) => p.SellingPrice;
        IQueryOptions<Product> queryOptions = new QueryOptions<Product>();

        // Act
        queryOptions.AddOrderDesc(orderBy);

        // Assert
        (Expression<Func<Product, object>>, bool) orderSpecification 
            = queryOptions.OrderExpressionsByDirection.First();
        
        Assert.That(queryOptions.OrderExpressionsByDirection, Has.Count.EqualTo(1));
        Assert.That(orderSpecification.Item1, Is.EqualTo(orderBy));
        Assert.That(orderSpecification.Item2, Is.False); // False is for desc order (bool isAsc = false)
    }

    [Test]
    public void AddOrder_StoresMultipleOrderExpressionsCorrectly()
    {
        // Arrange
        Expression<Func<Product, object>> firstOrder = (p) => p.Name;
        Expression<Func<Product, object>> secondOrder = (p) => p.SellingPrice;
        IQueryOptions<Product> queryOptions = new QueryOptions<Product>();

        // Act
        queryOptions.AddOrderAsc(firstOrder);
        queryOptions.AddOrderDesc(secondOrder);

        // Assert
        List<(Expression<Func<Product, object>>, bool)> orders 
            = queryOptions.OrderExpressionsByDirection.ToList();
        
        Assert.That(queryOptions.OrderExpressionsByDirection, Has.Count.EqualTo(2));
        
        Assert.That(orders[0].Item1, Is.EqualTo(firstOrder));
        Assert.That(orders[0].Item2, Is.True);
        
        Assert.That(orders[1].Item1, Is.EqualTo(secondOrder));
        Assert.That(orders[1].Item2, Is.False);
    }

    [Test]
    public void AsReadOnly_SetsIsReadonlyToTrue()
    {
        // Arrange
        IQueryOptions<Product> queryOptions = new QueryOptions<Product>();
        Assert.That(queryOptions.IsReadonly, Is.False);

        // Act
        queryOptions.AsReadOnly();

        // Assert
        Assert.That(queryOptions.IsReadonly, Is.True);
    }

    [Test]
    public void WithPagination_SetsPageAndLimitCorrectly()
    {
        // Arrange
        int page = 2;
        int limit = 10;
        IQueryOptions<Product> queryOptions = new QueryOptions<Product>();

        // Act
        queryOptions.WithPagination(page, limit);

        // Assert
        Assert.That(queryOptions.Page, Is.EqualTo(page));
        Assert.That(queryOptions.Limit, Is.EqualTo(limit));
    }

    [Test]
    public void WithPagination_SetsDefaultLimit_WhenLimitIsNotProvided()
    {
        // Arrange
        int page = 3;
        var queryOptions = new QueryOptions<Product>();

        // Act
        queryOptions.WithPagination(page);

        // Assert
        Assert.That(queryOptions.Page, Is.EqualTo(page));
        Assert.That(
            queryOptions.Limit,
            Is.EqualTo((int)TradeNest.GCommon.ApplicationConstants.DefaultPaginationLimitValue));
    }
}