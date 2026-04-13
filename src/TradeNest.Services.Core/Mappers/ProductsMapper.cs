using Riok.Mapperly.Abstractions;

using TradeNest.Data.Models;
using TradeNest.Services.Models.Image;
using TradeNest.Services.Models.Product;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Admin;

namespace TradeNest.Services.Core.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target,
    EnumMappingStrategy = EnumMappingStrategy.ByValue)]
public partial class ProductsMapper : IProductsMapper
{
    [MapProperty(nameof(Product.Category.Name), nameof(ProductDto.CategoryName))]
    [MapPropertyFromSource(nameof(ProductDto.FrontImageUrl), Use = nameof(MapFrontImageUrl))]
    public partial ProductDto ToProductDto(Product product);

    public partial IEnumerable<ProductDto> ToProductDtos(IEnumerable<Product> products);

    [MapProperty(nameof(Product.Owner.UserName), nameof(ProductDto2.OwnerName))]
    [MapProperty(nameof(Product.ApprovalDecision.ApprovalStatus), nameof(ProductDto2.ApprovalStatus))]
    public partial ProductDto2 ToProductDto2(Product product);

    public partial IEnumerable<ProductDto2> ToProductDtos2(IEnumerable<Product> product);

    #pragma warning disable RMG012
    [MapProperty(nameof(Product.ApprovalDecision), nameof(ProductDetailsDto.ApprovalDecision))]
    #pragma warning restore RMG012
    [MapProperty(nameof(Product.Owner.UserName), nameof(ProductDetailsDto.OwnerName))]
    [MapProperty(nameof(Product.Category.Name), nameof(ProductDetailsDto.CategoryName))]
    [MapPropertyFromSource(nameof(ProductDetailsDto.ApprovalDecision.ApprovalDecisionMakerDto), Use = nameof(MapApprovalDecisionMakerDto))]
    [MapPropertyFromSource(nameof(ProductDetailsDto.FrontImageUrl), Use = nameof(MapFrontImageUrl))]
    [MapPropertyFromSource(nameof(ProductDetailsDto.ImagesUrls), Use = nameof(MapImagesUrls))]
    public partial ProductDetailsDto ToProductDetailsDto(Product product, bool isOwner);
    
    [MapperIgnoreTarget(nameof(Product.Id))] [MapperIgnoreTarget(nameof(Product.Owner))]
    [MapperIgnoreTarget(nameof(Product.Category))] [MapperIgnoreTarget(nameof(Product.SoldProducts))]
    [MapperIgnoreTarget(nameof(Product.ProductCarts))] [MapperIgnoreTarget(nameof(Product.ProductWatchlists))]
    [MapperIgnoreTarget(nameof(Product.RowVersion))] [MapperIgnoreTarget(nameof(Product.ApprovalDecisionMaker))]
    [MapProperty(nameof(ProductCreateDto.ProductName), nameof(Product.Name))]
    [MapProperty(nameof(ProductCreateDto.CategoryId), nameof(Product.CategoryId))]
    [MapPropertyFromSource(nameof(Product.CreatedOn), Use = nameof(MapCreatedOn))]
    [MapPropertyFromSource(nameof(Product.IsDeleted), Use = nameof(MapIsDeleted))]
    public partial Product FromProductCreateDto(ProductCreateDto productCreateDto, Guid ownerId,
        IEnumerable<Image> images, Guid? approvalDecisionMakerId, ApprovalDecision approvalDecision);

    [MapperIgnoreTarget(nameof(ProductEditDto.FrontImageUrl))] 
    [MapperIgnoreTarget(nameof(ProductEditDto.NewImagesUrls))]
    [MapPropertyFromSource(nameof(ProductEditDto.ProductImages), Use = nameof(MapProductImages))]
    public partial ProductEditDto ToProductEditDto(Product product);

    [MapperIgnoreTarget(nameof(Product.Id))] [MapperIgnoreTarget(nameof(Product.Owner))]
    [MapperIgnoreTarget(nameof(Product.Category))] [MapperIgnoreTarget(nameof(Product.SoldProducts))]
    [MapperIgnoreTarget(nameof(Product.ProductCarts))] [MapperIgnoreTarget(nameof(Product.ProductWatchlists))] [MapperIgnoreTarget(nameof(Product.Images))]
    [MapperIgnoreTarget(nameof(Product.OwnerId))] [MapperIgnoreTarget(nameof(Product.IsDeleted))]
    [MapperIgnoreTarget(nameof(Product.CreatedOn))]
    public partial void EditProductFromProductEditDto(ProductEditDto productEditDto, Product product);

    private ApprovalDecisionMakerDto MapApprovalDecisionMakerDto(Product product)
    {
        return new ApprovalDecisionMakerDto()
        {
            ApprovalDecisionMakerAdminId = product.ApprovalDecisionMakerId,
            ApprovalDecisionMakerUsername = product.ApprovalDecisionMaker?.User.UserName,
            ApprovalDecisionMakerEmail = product.ApprovalDecisionMaker?.User.Email,
        };
    }
    
    private string? MapFrontImageUrl(Product product)
    {
        return product.Images
            .SingleOrDefault(i => i.IsFrontImage)?
            .Url ?? null;
    }

    private IEnumerable<string> MapImagesUrls(Product product)
    {
        return product.Images.Select(i => i.Url);
    }

    private DateTime MapCreatedOn(object _)
    {
        return DateTime.UtcNow;
    }

    private bool MapIsDeleted(object _)
    {
        return false;
    }

    private IEnumerable<ImageDto> MapProductImages(Product product)
    {
        return product.Images
            .Select(i => new ImageDto()
            {
                Id = i.Id,
                Url = i.Url,
            })
            .ToList();
    }
}