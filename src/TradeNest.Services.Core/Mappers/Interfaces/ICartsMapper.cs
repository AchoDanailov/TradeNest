using TradeNest.Data.Models;
using TradeNest.Services.Models.Cart;

namespace TradeNest.Services.Core.Mappers.Interfaces;

public interface ICartsMapper
{
    CartDto ToCartDto(Cart cart);
    CartProductDto ToCartProductDto(CartProduct cartProduct);
}