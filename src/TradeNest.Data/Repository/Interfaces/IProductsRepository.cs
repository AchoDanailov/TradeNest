using System.Linq.Expressions;
using TradeNest.Data.Models;
using TradeNest.Data.QueryOptions.Interfaces;

namespace TradeNest.Data.Repository.Interfaces;

public interface IProductsRepository : IReadRepository<Product>
{
    /// <summary>
    /// Provides the product details of the product with the given <paramref name="productId"/>.
    /// The data is loaded with the related products data: Category, Images, Owner, ApprovalDecisionMaker
    /// </summary>
    /// <param name="productId">The target product's identifier</param>
    /// <param name="asReadOnly">Boolean that indicates if the query should be optimized for data that will only be read.</param>
    /// <returns>
    /// A task containing product details of the product with the specified <paramref name="productId"/> with the related data. If no instance is found the operation returns null.
    /// </returns>
    Task<Product?> GetProductDetailsWithRelatedDataAsync(Guid productId, bool asReadOnly = false);

    Task<IEnumerable<Product>> GetAllInclArchivedAndNotApprovedAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null);
    
    Task<IEnumerable<Product>> GetAllInclNotApprovedAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null);
    
    /// <summary>
    /// Provides all categories bestseller products front images.
    /// </summary>
    /// <param name="asReadOnly">Boolean that indicates if the query should be optimized for data that will only be read.</param>
    /// <returns>
    /// A task containing a dictionary where the key is the Category Identifier as Guid and the value is the target category bestselling products front image url.
    /// </returns>
    Task<IDictionary<Guid, string?>> GetAllCategoriesBestSellersFrontImagesAsync(bool asReadOnly = false);

    Task<IEnumerable<Product>> GetAllProductsWithCategoryAndImagesAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null);

    /// <summary>
    /// Provides the number of products that match the query of the <paramref name="approved"/> and the
    /// <paramref name="search"/> parameters.
    /// </summary>
    /// <param name="approved">Boolean value representing whether the products being searched for should be approved or not.</param>
    /// <param name="search">
    /// String value used to search for products that contain this value either in their name or in their category name.
    /// </param>
    /// <returns>
    /// A task containing a number value representing how much matches have been found.
    /// </returns>
    Task<int> GetSpecifiedProductsCountAsync(bool? approved = null, string? search = null);

    Task<bool> ExistsIncludingArchivedAndNotApprovedAsync(Expression<Func<Product, bool>> filter);

    Task<bool> AddAsync(Product product);

    Task<bool> AddRangeAsync(IEnumerable<Product> products);

    Task<bool> UpdateAsync(Product product);

    /// <summary>
    /// Modifies all products categories that match the <paramref name="filter"/>
    /// to the category with the given identifier value <paramref name="newCategoryId"/>.
    /// </summary>
    /// <param name="filter">The filter for products.</param>
    /// <param name="newCategoryId">The category that the products categories will be changed.</param>
    /// <returns>
    /// A task containing a boolean value representing if one or more entities have been modified from the operation.
    /// </returns>
    /// <remarks>
    /// This method optimizes the update operation in any way possible, including not loading the entities in memory if possible. 
    /// </remarks>
    Task<bool> ExecuteUpdateProductsRangeCategoriesIdsAsync(
        Expression<Func<Product, bool>> filter,
        Guid newCategoryId);
    
    /// <summary>
    /// Soft deletes the given product.
    /// </summary>
    /// <param name="product">The product to be soft deleted.</param>
    /// <returns>A task containing a boolean value indicating if the operation was successful</returns>
    Task<bool> ArchiveAsync(Product product);
}
