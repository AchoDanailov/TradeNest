using TradeNest.Services.Models.Product;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Web.Mappers.Interfaces;

public interface IProductPresentationModelsMapper
{
    ProductViewModel ToProductViewModel(ProductDto productDto);
    IEnumerable<ProductViewModel> ToProductViewModels(IEnumerable<ProductDto> productDtos);

    ProductDetailsViewModel ToProductDetailsViewModel(ProductDetailsDto productDetailsDto,
        string returnUrl);

    ProductCreateDto FromProductCreateFormModel(ProductCreateFormModel productCreateFormModel);

    ProductEditFormModel ToProductEditFormModel(ProductEditDto productEditDto,
        List<AllCategoriesViewModel> allCategories, string returnUrl);
    ProductEditDto FromProductEditFormModel(ProductEditFormModel productEditFormModel);
}