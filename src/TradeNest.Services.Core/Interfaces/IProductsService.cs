using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models;
using TradeNest.Services.Models.Product;

namespace TradeNest.Services.Core.Interfaces;

public interface IProductsService
{
    /// <summary>
    /// Retrieves all products ordered by creation date (newest first).
    /// </summary>
    /// <param name="search">
    /// String value used to search for products that contain this value
    /// either in their name or in their category.
    /// </param>
    /// <returns>A task that returns the collection of products, empty if none exist.</returns>
    Task<IEnumerable<ProductDto>> GetAllProductsOrderedByDateOfCreationDescAsync(
        string? search = null);

    /// <summary>
    /// Retrieves all products that belong to the specified category.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="search">
    /// String value used to search for products that contain this value
    /// either in their name or in their category.
    /// </param>
    /// <returns>A task that returns the collection of matching products, empty if none found.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="categoryId"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    Task<IEnumerable<ProductDto>> GetAllProductsByCategoryIdAsync(Guid categoryId,
        string? search = null);

    /// <summary>
    /// Retrieves all products ordered by number of orders (highest first).
    /// </summary>
    /// <returns>A task that returns the collection of products ordered by popularity.</returns>
    Task<IEnumerable<ProductDto>> GetAllProductsOrderedBySellingCountDescAsync();

    /// <summary>
    /// Determines whether a product with the specified identifier exists.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task that returns true if the product exists; otherwise, false.</returns>
    Task<bool> ProductExistsByIdAsync(Guid id);

    /// <summary>
    /// Retrieves detailed information for the specified product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="userId">
    /// The identifier of the user that wants to view the product details.
    /// In case of non authenticated user userId can be left null.
    /// </param>
    /// <returns>A task that returns the product details, or null if not found.</returns>
    Task<ProductDetailsDto?> GetProductDetailsByIdAsync(Guid id,
        Guid? userId = null);

    /// <summary>
    /// Creates a new product and saves it in the data store.
    /// </summary>
    /// <param name="userId">The identifier of the user creating the product.</param>
    /// <param name="productCreateDto">The product data.</param>
    /// <returns>A task that returns the identifier of the newly created product.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId"/>, categoryId in the
    /// <paramref name="productCreateDto"/> are empty or entities with the
    /// corresponding identificators do not exist
    /// </exception>
    Task<Guid> CreateProductAsync(Guid userId, ProductCreateDto productCreateDto);

    /// <summary>
    /// Retrieves a product form model populated with existing data for editing.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task that returns the form model, or null if the product is not found.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the <paramref name="userId"/> is with value <see cref="Guid.Empty"/> or user
    /// was not found with the specified identitficator.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown when user with <paramref name="userId"/> is not the owner of the product with
    /// the provided <paramref name="id"/>.
    /// </exception>
    Task<ProductEditDto?> GetProductForEditAsync(Guid userId, Guid id);

    /// <summary>
    /// Deletes the specified product.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId"/> and <paramref name="id"/> are
    /// with value <see cref="Guid.Empty"/> or user with the specified identifier
    /// <paramref name="userId"/> does not exist.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if the product with id <paramref name="id"/> does not exist.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the user with the provided <paramref name="userId"/> is not the owner
    /// of the product with the provided <paramref name="id"/>.
    /// </exception>
    Task DeleteProductAsync(Guid userId, Guid id);

    /// <summary>
    /// Updates the specified product with the provided data.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="productEditDto">The updated product data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if any of the following: <paramref name="userId"/>,
    /// <paramref name="productEditDto.ProductId"/> or
    /// <paramref name="productEditDto.CategoryId"/> is with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if user with <paramref name="userId"/> do not exist. Or if any of the provided
    /// ProductImages in <paramref name="productEditDto.ProductImages"/> are not images
    /// of the product.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown if product with the provided product id value in
    /// <paramref name="productEditDto.ProductId"/>, does not exist.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown when user with <paramref name="userId"/> is not the owner of the product.
    /// </exception>
    Task EditProductAsync(Guid userId, ProductEditDto productEditDto);
}