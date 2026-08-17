using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Cart;
using TradeNest.GCommon.Exceptions;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.Models.Cart;

namespace TradeNest.Web.Controllers;

public class CartApiController : BaseApiController
{
    private readonly ICartsService _cartsService;
    private readonly ICartPresentationModelsMapper _cartMapper;

    public CartApiController(ICartsService cartsService, 
        ICartPresentationModelsMapper cartMapper)
    {
        this._cartsService = cartsService;
        this._cartMapper = cartMapper;
    }

    [HttpPut]
    [Route("cart/{cartId}")]
    [ProducesResponseType(StatusCodes.Status200OK)] [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)][ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> UpdateCartProduct(
        [FromRoute] Guid cartId,
        [FromQuery] Guid productId,
        [FromBody] UpdateCartProductRequestDto updateCartProductRequestDto)
    {
        if (updateCartProductRequestDto.CartId != cartId ||
            updateCartProductRequestDto.ProductId != productId)
        {
            return BadRequest();
        }

        try
        {
            Guid userId = this.GetUserId(throwIfNull: true);
            UpdateCartProductDto updateCartProductDto = this._cartMapper
                .ToUpdateCartProductDto(updateCartProductRequestDto);

            bool isSuccess = await this._cartsService.UpdateCartProductAsync(userId, updateCartProductDto);
            return Ok(isSuccess);
        }
        catch (InsufficientProductQuantityInStockException)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("cart/addProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)] [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> AddProductToCart(
        [FromBody] AddProductToCartRequestDto addProductToCartRequestDto)
    {
        if (addProductToCartRequestDto.ProductId == Guid.Empty)
            return BadRequest();

        try
        {
            Guid userId = this.GetUserId(throwIfNull: true);
            await this._cartsService.AddProductToCartAsync(userId,
                addProductToCartRequestDto.ProductId, addProductToCartRequestDto.Quantity);

            return Ok(true);
        }
        catch (InsufficientProductQuantityInStockException)
        {
            return BadRequest();
        }
        catch (ProductDisabledException)
        {
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("cart/cartProducts/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)] [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CartProductResponseDto>> GetCartProductData(Guid productId)
    {
        if (productId == Guid.Empty)
            return BadRequest();

        Guid userId = this.GetUserId(throwIfNull: true);

        CartProductDto? cartProductDto = await this._cartsService
            .GetCartProductDataByUserIdAndProductIdAsync(userId, productId);
        if (cartProductDto == null)
            return Ok(new CartProductResponseDto());

        CartProductResponseDto cartProductResponseDto = this._cartMapper
            .ToCartProductResponseDto(cartProductDto);
        return Ok(cartProductResponseDto);
    }
}