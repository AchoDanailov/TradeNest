using TradeNest.Services.Models.Cart;
using TradeNest.Web.Models.Cart;

namespace TradeNest.Web.Mappers.Interfaces;

public interface ICartPresentationModelsMapper
{
    CartViewModel ToCartViewModel(CartDto cartDto);

    CartProductViewModel ToCartProductViewModel(CartProductDto cartProductDto);
    IEnumerable<CartProductViewModel> ToCartProductViewModels(IEnumerable<CartProductDto> cartProductDto);

    UpdateCartProductDto ToUpdateCartProductDto(UpdateCartProductRequestDto cartProductRequestDto);

    CartProductResponseDto ToCartProductResponseDto(CartProductDto cartProductDto);
}