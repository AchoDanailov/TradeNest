using TradeNest.Web.ViewModels.Product;
using TradeNest.GCommon.Exceptions;

namespace TradeNest.Services.Core.Interfaces;

public interface IProductsService
{
    /// <summary>
    /// Creates an empty catalog container for products and categories.
    /// </summary>
    /// <param name="isFromSearchInput">
    /// Indicates whether the data is requested as part of a search operation.
    /// </param>
    /// <returns>An empty catalog containing products and categories.</returns>
    CatalogProductsAndCategoriesViewModel GetEmptyCatalogProdsAndCategoriesDto(
        bool isFromSearchInput = false);

    /// <summary>
    /// Creates a catalog container with categories loaded and optional products included.
    /// </summary>
    /// <param name="productsViewModels">
    /// Optional collection of products to include.
    /// </param>
    /// <param name="isFromSearchInput">
    /// Indicates whether the data is requested as part of a search operation.
    /// </param>
    /// <returns>
    /// A task that returns a catalog containing products and categories.
    /// </returns>
    Task<CatalogProductsAndCategoriesViewModel> GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
        IEnumerable<ProductViewModel>? productsViewModels = null,
        bool isFromSearchInput = false);

    /// <summary>
    /// Retrieves all products ordered by creation date (newest first).
    /// </summary>
    /// <returns>A task that returns the collection of products, empty if none exist.</returns>
    Task<IEnumerable<ProductViewModel>> GetAllProductsOrderedByDateOfCreationDescAsync();

    /// <summary>
    /// Retrieves all products whose name contains the given search query.
    /// </summary>
    /// <param name="searchQuery">The search term.</param>
    /// <returns>A task that returns matching products, empty if none found.</returns>
    Task<IEnumerable<ProductViewModel>> GetAllProdsBySearchQueryForNameAsync(
        string searchQuery);

    /// <summary>
    /// Retrieves all products that belong to the specified category.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <returns>A task that returns the collection of matching products, empty if none found.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="categoryId"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryIdAsync(Guid categoryId);

    /// <summary>
    /// Retrieves all products ordered by number of orders (highest first).
    /// </summary>
    /// <returns>A task that returns the collection of products ordered by popularity.</returns>
    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsOrderedByOrdersCountDescAsync();

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
    Task<ProductDetailsViewModel?> GetProductDetailsViewModelByIdAsync(Guid id,
        Guid? userId = null);

    /// <summary>
    /// Creates an empty form model for product creation.
    /// </summary>
    /// <returns>An empty product creation form.</returns>
    ProductCreateFormModel GetEmptyProductCreateFormModel();

    /// <summary>
    /// Creates a product form model with all categories loaded.
    /// </summary>
    /// <returns>A task that returns a populated product creation form.</returns>
    Task<ProductCreateFormModel> GetProdCreateFormModelWithLoadedCategoriesAsync();

    /// <summary>
    /// Creates a new product and saves it in the data store.
    /// </summary>
    /// <param name="userId">The identifier of the user creating the product.</param>
    /// <param name="productCreateFormModel">The product data.</param>
    /// <returns>A task that returns the identifier of the newly created product.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId"/>, categoryId in the
    /// <paramref name="productCreateFormModel"/> are empty or entities with the
    /// corresponding identificators do not exist
    /// </exception>
    Task<Guid> CreateProductAsync(Guid userId, ProductCreateFormModel productCreateFormModel);

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
    Task<ProductEditFormModel?> GetProductEditFormModelAsync(Guid userId, Guid id);

    /// <summary>
    /// Deletes the specified product.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="userId"/> and <paramref name="id"/> are
    /// with value <see cref="Guid.Empty"/> or entities with the specified identifiers
    /// do not exist.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the user with the provided <paramref name="userId"/> is not the owner
    /// of the product with the provided <paramref name="id"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the product is already deleted.
    /// </exception>
    Task DeleteProductAsync(Guid userId, Guid id);

    /// <summary>
    /// Updates the specified product with the provided data.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="productEditFormModel">The updated product data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if any of the following: <paramref name="userId"/>,
    /// <paramref name="productEditFormModel.ProductId"/> or
    /// <paramref name="productEditFormModel.CategoryId"/> is with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if product with <paramref name="productEditFormModel.ProductId"/> or user with
    /// <paramref name="userId"/> do not exist. Or if any of the provided ProductImages in
    /// <paramref name="productEditFormModel.ProductImages"/> are not images of the product.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown when user with <paramref name="userId"/> is not the owner of the product.
    /// </exception>
    Task EditProductAsync(Guid userId, ProductEditFormModel productEditFormModel);
}