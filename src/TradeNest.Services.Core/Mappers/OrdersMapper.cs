using Riok.Mapperly.Abstractions;

using TradeNest.Data.Models;
using TradeNest.Services.Models.Order;
using TradeNest.Services.Core.Mappers.Interfaces;

namespace TradeNest.Services.Core.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrdersMapper : IOrdersMapper
{
    public partial OrderDto ToOrderDto(Order order);
    public partial IEnumerable<OrderDto> ToOrderDtos(IEnumerable<Order> orders);
    
    [MapProperty(nameof(OrderProduct.ProductNameAtOrderTime), nameof(OrderProductDto.Name))]
    [MapProperty(nameof(OrderProduct.UnitSellingPriceAtOrderTime), nameof(OrderProductDto.UnitPriceAtOrderTime))]
    [MapProperty(nameof(OrderProduct.TotalProductPriceAtOrderTime), nameof(OrderProductDto.TotalPriceAtOrderTime))]
    public partial OrderProductDto ToOrderProductDto(OrderProduct orderProduct);

    [MapperIgnoreTarget(nameof(OrderProduct.Id))] [MapperIgnoreTarget(nameof(OrderProduct.OrderId))] [MapperIgnoreTarget(nameof(OrderProduct.Order))]
    [MapProperty(nameof(CartProduct.ProductId), nameof(OrderProduct.OriginalProductId))]
    [MapProperty(nameof(CartProduct.Product), nameof(OrderProduct.OriginalProduct))]
    [MapProperty(nameof(CartProduct.Product.Name), nameof(OrderProduct.ProductNameAtOrderTime))]
    [MapProperty(nameof(CartProduct.Product.CostPrice), nameof(OrderProduct.CostPriceAtOrderTime))]
    [MapProperty(nameof(CartProduct.Product.SellingPrice), nameof(OrderProduct.UnitSellingPriceAtOrderTime))]
    [MapProperty(nameof(CartProduct.ProductQuantityAdded), nameof(OrderProduct.QuantityOrdered))]
    [MapPropertyFromSource(nameof(OrderProduct.TotalProductPriceAtOrderTime), Use = nameof(MapTotalProductPriceAtOrderTime))]
    public partial OrderProduct OrderProductFromCartProduct(CartProduct cartProduct);

    private decimal MapTotalProductPriceAtOrderTime(CartProduct cartProduct)
    {
        return cartProduct.Product.SellingPrice * cartProduct.ProductQuantityAdded;
    }
}