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
}