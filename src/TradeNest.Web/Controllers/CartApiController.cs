using Microsoft.AspNetCore.Mvc;

using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Cart;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.ViewModels;
using static TradeNest.Web.Utilities.Messages.LoggingErrorMessages;

namespace TradeNest.Web.Controllers;

public class CartApiController : BaseApiController
{
    private readonly ICartsService _cartsService;
    private readonly ICartPresentationModelsMapper _cartMapper;
    private readonly ILogger<CartApiController> _logger;

    public CartApiController(ICartsService cartsService, 
        ICartPresentationModelsMapper cartMapper, ILogger<CartApiController> logger)
    {
        this._cartsService = cartsService;
        this._cartMapper = cartMapper;
        this._logger = logger;
    }

    [HttpPut]
    [Route("cart/{cartId}")]
    [ProducesResponseType(StatusCodes.Status200OK)] [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            bool isSuccess = await this._cartsService.UpdateCartProduct(userId, updateCartProductDto);
            return Ok(isSuccess);
        }
        catch (InsufficientProductQuantityInStockException)
        {
            return BadRequest();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedOperationException)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                string.Format(
                    DefaultLogExceptionMessageWithControllerAndAction,
                    ex.GetType().Name,
                    this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName));
            
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}