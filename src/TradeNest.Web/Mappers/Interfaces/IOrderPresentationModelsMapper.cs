using TradeNest.Services.Models.Order;
using TradeNest.Web.Models.Order;

namespace TradeNest.Web.Mappers.Interfaces;

public interface IOrderPresentationModelsMapper
{
    OrderViewModel ToOrderViewModel(OrderDto orderDto);
    IEnumerable<OrderViewModel> ToOrderViewModels(IEnumerable<OrderDto> orderDtos);

    OrderProductViewModel ToOrderProductViewModel(OrderProductDto orderProductDto);
    IEnumerable<OrderProductViewModel> ToOrderProductViewModels(IEnumerable<OrderProductDto> orderProductDtos);
}