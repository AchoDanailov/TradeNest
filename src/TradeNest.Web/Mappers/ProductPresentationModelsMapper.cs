using Riok.Mapperly.Abstractions;

using TradeNest.Services.Models.Image;
using TradeNest.Services.Models.Product;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Image;
using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Web.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductPresentationModelsMapper : IProductPresentationModelsMapper
{
    public partial ProductViewModel ToProductViewModel(ProductDto productDto);
    
    public partial IEnumerable<ProductViewModel> ToProductViewModels(
        IEnumerable<ProductDto> productDtos);

    public partial ProductDetailsViewModel ToProductDetailsViewModel(
        ProductDetailsDto productDetailsDto, string returnUrl);
    
    public partial ProductCreateDto FromProductCreateFormModel(
        ProductCreateFormModel productCreateFormModel);

    [MapProperty(nameof(ProductEditDto.Id), nameof(ProductEditFormModel.ProductId))]
    [MapProperty(nameof(ProductEditDto.Name), nameof(ProductEditFormModel.ProductName))]
    [MapPropertyFromSource(nameof(ProductEditFormModel.ProductImages), Use = nameof(MapProductImages))]
    public partial ProductEditFormModel ToProductEditFormModel(ProductEditDto productEditDto, 
        List<AllCategoriesViewModel> allCategories, string returnUrl);

    [MapProperty(nameof(ProductEditFormModel.ProductId), nameof(ProductEditDto.Id))]
    [MapProperty(nameof(ProductEditFormModel.ProductName), nameof(ProductEditDto.Name))]
    [MapPropertyFromSource(nameof(ProductEditDto.ProductImages), Use = nameof(MapProductImagesToDtos))]
    [MapperIgnoreTarget(nameof(ProductEditDto.CategoryName))]
    [MapperIgnoreTarget(nameof(ProductEditDto.FrontImageUrl))]
    [MapperIgnoreTarget(nameof(ProductEditDto.OwnerId))]
    public partial ProductEditDto FromProductEditFormModel(ProductEditFormModel productEditFormModel);
    
    public partial ProductResponseDto ToProductResponseDto(ProductDetailsDto productDetailsDto);

    private List<ImageViewModel> MapProductImages(ProductEditDto productEditDto)
    {
        return productEditDto.ProductImages
            .Select(i => new ImageViewModel()
            {
                Id = i.Id,
                Url = i.Url,
                IsMarkedToStay = i.IsMarkedToStay,
            })
            .ToList();
    }

    private IEnumerable<ImageDto> MapProductImagesToDtos(ProductEditFormModel productEditFormModel)
    {
        return productEditFormModel.ProductImages
            .Select(i => new ImageDto()
            {
                Id = i.Id,
                Url = i.Url,
                IsMarkedToStay = i.IsMarkedToStay,
            });
    }
}