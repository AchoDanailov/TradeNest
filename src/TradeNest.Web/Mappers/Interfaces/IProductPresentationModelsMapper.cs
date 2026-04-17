using TradeNest.Services.Models.Product;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Web.Mappers.Interfaces;

public interface IProductPresentationModelsMapper
{
    ProductViewModel ToProductViewModel(ProductDto productDto);
    IEnumerable<ProductViewModel> ToProductViewModels(IEnumerable<ProductDto> productDtos);

    ProductResponseDto ToProductResponseDto(ProductDto2 productDto);
    IEnumerable<ProductResponseDto> ToProductResponseDtos(IEnumerable<ProductDto2> productDtos);

    ProductDetailsViewModel ToProductDetailsViewModel(ProductDetailsDto productDetailsDto,
        string returnUrl);

    ProductCreateDto FromProductCreateFormModel(ProductCreateFormModel productCreateFormModel);

    ProductEditFormModel ToProductEditFormModel(ProductEditDto productEditDto,
        List<AllCategoriesViewModel> allCategories, string returnUrl);
    ProductEditDto FromProductEditFormModel(ProductEditFormModel productEditFormModel);
    
    ProductDetailsResponseDto ToProductResponseDto(ProductDetailsDto productDetailsDto);
    
    EditApprovalDecisionDto FromEditProductApprovalStatusRequestDto(
        EditProductApprovalStatusRequestDto requestDto);
}