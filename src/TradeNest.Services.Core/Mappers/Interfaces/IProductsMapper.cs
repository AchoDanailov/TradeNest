using TradeNest.Data.Models;
using TradeNest.Services.Models.Product;

namespace TradeNest.Services.Core.Mappers.Interfaces;

public interface IProductsMapper
{
    ProductDto ToProductDto(Product product);
    IEnumerable<ProductDto> ToProductDtos(IEnumerable<Product> products);
    
     ProductWithApprovalStatusDto ToProductWithApprovalStatusDto(Product product);
     IEnumerable<ProductWithApprovalStatusDto> ToProductWithApprovalStatusDtos(IEnumerable<Product> product);

    ProductDetailsDto ToProductDetailsDto(Product product, bool isOwner);

    Product FromProductCreateDto(ProductCreateDto productCreateDto, Guid ownerId,
        IEnumerable<Image> images, Guid? approvalDecisionMakerId, ApprovalDecision approvalDecision);

    ProductEditDto ToProductEditDto(Product product);
    void EditProductFromProductEditDto(ProductEditDto productEditDto, Product product);

    SellerProductDto ToSellerProductDto(Product product);
    IEnumerable<SellerProductDto> ToSellerProductDtos(IEnumerable<Product> products);
}