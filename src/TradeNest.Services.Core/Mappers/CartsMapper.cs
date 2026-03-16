using Riok.Mapperly.Abstractions;

using TradeNest.Data.Models;
using TradeNest.Services.Models.Cart;
using TradeNest.Services.Core.Mappers.Interfaces;

namespace TradeNest.Services.Core.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartsMapper : ICartsMapper
{
    [MapProperty(nameof(Cart.Id), nameof(CartDto.CartId))]
    [MapPropertyFromSource(nameof(CartDto.TotalPrice), Use = nameof(MapTotalPrice))]
    public partial CartDto ToCartDto(Cart cart);

    [MapProperty(nameof(CartProduct.ProductId), nameof(CartProductDto.Id))]
    [MapProperty(nameof(CartProduct.Product.Name), nameof(CartProductDto.Name))]
    [MapProperty(nameof(CartProduct.ProductQuantityAdded), nameof(CartProductDto.QuantityAdded))]
    [MapProperty(nameof(CartProduct.Product.SellingPrice), nameof(CartProductDto.UnitPrice))]
    [MapPropertyFromSource(nameof(CartProductDto.TotalPrice), Use = nameof(MapCartProductTotalPrice))]
    [MapProperty(nameof(CartProduct.Product.IsEnabled), nameof(CartProductDto.IsEnabled))]
    [MapPropertyFromSource(nameof(CartProductDto.IsEnoughQtyLeft), Use = nameof(MapIsEnoughQtyLeft))]
    public partial CartProductDto ToCartProductDto(CartProduct cartProduct);

    private decimal MapTotalPrice(Cart cart)
    {
        return cart.CartProducts
            .Select(cp => cp.ProductQuantityAdded * cp.Product.SellingPrice)
            .Sum();
    }

    private decimal MapCartProductTotalPrice(CartProduct cartProduct)
    {
        return cartProduct.ProductQuantityAdded * cartProduct.Product.SellingPrice;
    }

    private bool MapIsEnoughQtyLeft(CartProduct cartProduct)
    {
        return cartProduct.Product.QuantityInStock >= cartProduct.ProductQuantityAdded;
    }
}