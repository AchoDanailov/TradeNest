using Riok.Mapperly.Abstractions;

using TradeNest.Services.Models.Order;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.ViewModels.Order;

namespace TradeNest.Web.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrderPresentationModelsMapper : IOrderPresentationModelsMapper
{
    public partial OrderViewModel ToOrderViewModel(OrderDto orderDto);
    public partial IEnumerable<OrderViewModel> ToOrderViewModels(IEnumerable<OrderDto> orderDtos);

    [MapProperty(nameof(OrderProductDto.TotalPriceAtOrderTime), nameof(OrderProductViewModel.TotalPrice))]
    [MapProperty(nameof(OrderProductDto.UnitPriceAtOrderTime), nameof(OrderProductViewModel.UnitPrice))]
    public partial OrderProductViewModel ToOrderProductViewModel(OrderProductDto orderProductDto);
    public partial IEnumerable<OrderProductViewModel> ToOrderProductViewModels(IEnumerable<OrderProductDto> orderProductDtos);
}