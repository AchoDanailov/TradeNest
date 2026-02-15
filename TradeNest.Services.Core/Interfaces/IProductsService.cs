using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Services.Core.Interfaces;

public interface IProductsService
{
    CatalogProductsAndCategoriesViewModel GetEmptyCatalogProdsAndCategoriesDto();

    Task<CatalogProductsAndCategoriesViewModel> GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
        IEnumerable<ProductViewModel>? productsViewModels = null);

    Task<IEnumerable<ProductViewModel>> GetAllProductVmsOrderedByCreatedOnAsync();

    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsWithSearchQueryForNameAsync(
        string searchQuery);

    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryAsync(Guid categoryId);

    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsOrderedByOrdersCountDescAsync();

    Task<bool> ProductExists(Guid id);

    Task<ProductDetailsViewModel?> GetProductDetailsViewModelById(Guid id);

    ProductCreateFormModel GetEmptyProductCreateFormModel();

    Task<ProductCreateFormModel> GetProdCreateFormModelWithLoadedCategoriesAsync();
}