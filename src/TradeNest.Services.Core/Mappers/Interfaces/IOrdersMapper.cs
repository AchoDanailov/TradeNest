using TradeNest.Data.Models;
using TradeNest.Services.Models.Order;

namespace TradeNest.Services.Core.Mappers.Interfaces;

public interface IOrdersMapper
{
    OrderDto ToOrderDto(Order order);
    IEnumerable<OrderDto> ToOrderDtos(IEnumerable<Order> orders);
    
    OrderProductDto ToOrderProductDto(OrderProduct orderProduct);
}