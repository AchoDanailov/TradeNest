using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Services.Core.Interfaces;

public interface IProductsService
{
    /// <summary>
    /// Provides an empty instance of the CatalogProductsAndCategoriesViewModel type.
    /// </summary>
    /// <param name="isFromSearchInput">
    /// This parameter specifies weather the resources are requested from a search
    /// operation or not
    /// </param>
    /// <returns>an empty CatalogProductsAndCategoriesViewModel instance</returns>
    CatalogProductsAndCategoriesViewModel GetEmptyCatalogProdsAndCategoriesDto(bool isFromSearchInput = false);

    /// <summary>
    /// Provides a CatalogProductsAndCategoriesViewModel instance with the relevant
    /// AllCategoriesViewModel collection. 
    /// </summary>
    /// <param name="productsViewModels">
    /// Collection with ProductViewModel types to be added in the
    /// CatalogProductsAndCategoriesViewModel instance.
    /// </param>
    /// <param name="isFromSearchInput">
    /// This parameter specifies weather the resources are requested from a search
    /// operation or not
    /// </param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The Task result contains a CatalogProductsAndCategoriesViewModel instance.
    /// </returns>
    Task<CatalogProductsAndCategoriesViewModel> GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
        IEnumerable<ProductViewModel>? productsViewModels = null, bool isFromSearchInput = false);

    /// <summary>
    /// Provides a collection with all ProductViewModels ordered by date of creation
    /// in descending order.
    /// </summary>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The Task result contains a ProductViewModel collection.
    /// </returns>
    /// <remarks>If no products are found the method returns an empty collection.</remarks>
    Task<IEnumerable<ProductViewModel>> GetAllProductVmsOrderedByCreatedOnDescAsync();

    /// <summary>
    /// Provides a collection with all ProductViewModels that have Name that contains
    /// the given searchQuery parameter's value.
    /// </summary>
    /// <param name="searchQuery">The value used when searching for a match.</param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains a ProductViewModel collection that contains the matched
    /// products if any.
    /// </returns>
    /// <remarks>If no matches are found the method returns an empty collection.</remarks>
    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsWithSearchQueryForNameAsync(
        string searchQuery);

    /// <summary>
    /// Provides a collection with all ProductViewModels that have category
    /// with id equal to the given categoryId parameter's value.
    /// </summary>
    /// <param name="categoryId">The value used when searching for a match.</param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task contains a ProductViewModel collection that contains the
    /// matched products if any.
    /// </returns>
    /// <remarks>If no matches are found the method returns an empty collection.</remarks>
    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryAsync(Guid categoryId);

    /// <summary>
    /// Provides a collection with all ProductViewModels ordered by the products orders count
    /// in descending order.
    /// </summary>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains a ProductViewModel collection ordered by orders count
    /// in descending order.
    /// </returns>
    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsOrderedByOrdersCountDescAsync();

    /// <summary>
    /// Checks if the product exists by the given id parameter's value
    /// </summary>
    /// <param name="id">The value used to find a match with.</param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains a value representing weather the product with the
    /// given id exists.
    /// </returns>
    /// <remarks>This method makes a connection to the given data store.</remarks>
    Task<bool> ProductExistsAsync(Guid id);

    /// <summary>
    /// Gets the ProductDetailsViewModel instance for the product with id equal to the given
    /// id parameter's value.
    /// </summary>
    /// <param name="id">The value used to find a match with.</param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains the corresponding ProductDetailsViewModel instance.
    /// </returns>
    /// <remarks>If the method can not find a match it returns null.</remarks>
    Task<ProductDetailsViewModel?> GetProductDetailsViewModelByIdAsync(Guid id);

    /// <summary>
    /// Provides an empty instance of the ProductCreateFormModel type.
    /// </summary>
    /// <returns>an empty ProductCreateFormModel instance</returns>
    ProductCreateFormModel GetEmptyProductCreateFormModel();

    /// <summary>
    /// Provides a ProductCreateFormModel instance with the relevant AllCategoriesViewModel
    /// collection. 
    /// </summary>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains a ProductCreateFormModel instance with the loaded
    /// categories
    /// </returns>
    Task<ProductCreateFormModel> GetProdCreateFormModelWithLoadedCategoriesAsync();

    /// <summary>
    /// Creates a new Product instance. And saves it in the data store.
    /// </summary>
    /// <param name="userId">The user's id creating the product.</param>
    /// <param name="productCreateFormModel">
    /// The product form model that contains the data for the product creation.
    /// </param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains the string representation of the newly created product.
    /// </returns>
    Task<string> CreateProductAsync(Guid userId, ProductCreateFormModel productCreateFormModel);

    /// <summary>
    /// Provides a ProductEditFormModel with populated product data for edit that
    /// corresponds to the product with id equal to the given id parameter.
    /// </summary>
    /// <param name="id">The product's id</param>
    /// <param name="userId">The user's identifier trying to perform the operation</param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains the corresponding ProductEditFormModel instance.
    /// </returns>
    /// <remarks>If no product is found with the given id the method returns null.</remarks>
    Task<ProductEditFormModel?> GetProductEditFormModelAsync(Guid userId, Guid id);
    
    /// <summary>
    /// The operation deletes a product with the given id from the data store.
    /// </summary>
    /// <param name="id">The product's identifier.</param>
    /// <param name="userId">The user's identifier trying to perform the operation.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    Task DeleteProductAsync(Guid userId, Guid id);

    /// <summary>
    /// The operation makes changes to the product, specified in the given
    /// ProductEditFormModel instance.
    /// </summary>
    /// <param name="userId">The user's identifier trying to perform the operation.</param>
    /// <param name="productId">The product's identifier.</param>
    /// <param name="productEditFormModel">The instance carrying the changes that should be persisted.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    Task EditProductAsync(Guid userId, Guid productId, ProductEditFormModel productEditFormModel);
}