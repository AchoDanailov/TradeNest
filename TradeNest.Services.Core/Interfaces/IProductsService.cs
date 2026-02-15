using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Services.Core.Interfaces;

public interface IProductsService
{
    CatalogProductsAndCategoriesViewModel CreateEmptyCatalogProdsAndCategoriesDto();

    Task<CatalogProductsAndCategoriesViewModel> GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
        IEnumerable<ProductViewModel>? productsViewModels = null);

    Task<IEnumerable<ProductViewModel>> GetAllProductVmsOrderedByCreatedOnAsync();

    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsWithSearchQueryForNameAsync(
        string searchQuery);

    Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryAsync(Guid categoryId);
}