using Riok.Mapperly.Abstractions;

using TradeNest.Services.Models.Cart;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.Models.Cart;

namespace TradeNest.Web.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartPresentationModelsMapper : ICartPresentationModelsMapper
{
    public partial CartViewModel ToCartViewModel(CartDto cartDto);

    public partial CartProductViewModel ToCartProductViewModel(CartProductDto cartProductDto);
    public partial IEnumerable<CartProductViewModel> ToCartProductViewModels(IEnumerable<CartProductDto> cartProductDto);

    public partial UpdateCartProductDto ToUpdateCartProductDto(UpdateCartProductRequestDto cartProductRequestDto);
    
    public partial CartProductResponseDto ToCartProductResponseDto(CartProductDto cartProductDto);
}