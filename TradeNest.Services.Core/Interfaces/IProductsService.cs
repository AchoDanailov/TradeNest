using TradeNest.Web.ViewModels.Product;

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
    /// <returns>A task that returns the product details, or null if not found.</returns>
    Task<ProductDetailsViewModel?> GetProductDetailsViewModelByIdAsync(Guid id);

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
    Task<Guid> CreateProductAsync(Guid userId, ProductCreateFormModel productCreateFormModel);

    /// <summary>
    /// Retrieves a product form model populated with existing data for editing.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task that returns the form model, or null if the product is not found.</returns>
    Task<ProductEditFormModel?> GetProductEditFormModelAsync(Guid userId, Guid id);

    /// <summary>
    /// Deletes the specified product.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteProductAsync(Guid userId, Guid id);

    /// <summary>
    /// Updates the specified product with the provided data.
    /// </summary>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    /// <param name="productId">The product identifier.</param>
    /// <param name="productEditFormModel">The updated product data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EditProductAsync(Guid userId,
        Guid productId, ProductEditFormModel productEditFormModel);
}