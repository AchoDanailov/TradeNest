using TradeNest.Services.Models.Product;
using TradeNest.Web.Models.Category;
using TradeNest.Web.Models.MyNest;
using TradeNest.Web.Models.Product;

namespace TradeNest.Web.Mappers.Interfaces;

public interface IProductPresentationModelsMapper
{
    ProductViewModel ToProductViewModel(ProductDto productDto);
    IEnumerable<ProductViewModel> ToProductViewModels(IEnumerable<ProductDto> productDtos);

    ProductResponseDto ToProductResponseDto(ProductWithApprovalStatusDto productDto);
    IEnumerable<ProductResponseDto> ToProductResponseDtos(IEnumerable<ProductWithApprovalStatusDto> productDtos);

    ProductDetailsViewModel ToProductDetailsViewModel(ProductDetailsDto productDetailsDto,
        string returnUrl);

    ProductCreateDto FromProductCreateFormModel(ProductCreateFormModel productCreateFormModel);

    ProductEditFormModel ToProductEditFormModel(ProductEditDto productEditDto,
        List<AllCategoriesViewModel> allCategories, string returnUrl);
    ProductEditDto FromProductEditFormModel(ProductEditFormModel productEditFormModel);
    
    ProductDetailsResponseDto ToProductResponseDto(ProductDetailsDto productDetailsDto);
    
    EditApprovalDecisionDto FromEditProductApprovalStatusRequestDto(
        EditProductApprovalStatusRequestDto requestDto);

    SellerProductViewModel ToSellerProductViewModel(SellerProductDto sellerProductDto);
    IEnumerable<SellerProductViewModel> ToSellerProductViewModels(
        IEnumerable<SellerProductDto> sellerProductDtos);
}