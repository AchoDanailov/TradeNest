using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Cart;
using TradeNest.Services.Models.Order;
using TradeNest.Services.Models.Product;
using TradeNest.Web.ViewModels.Order;
using TradeNest.Web.ViewModels.Cart;
using TradeNest.Web.ViewModels.Product;
using static TradeNest.Web.Utilities.Messages.StatusNotificationMessages;
using static TradeNest.Web.Utilities.Messages.LoggingErrorMessages;

namespace TradeNest.Web.Controllers;

[Authorize]
public class CartController : BaseController
{
    private readonly ILogger<CartController> _logger;
    private readonly IOrdersService _ordersService;
    private readonly ICartsService _cartsService;

    public CartController(IOrdersService ordersService, ICartsService cartsService,
        ILogger<CartController> logger)
    {
        this._logger = logger;
        this._ordersService = ordersService;
        this._cartsService = cartsService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        Guid userId = this.GetUserId(throwIfNull: true);
        CartWithOrdersViewModel viewModel 
            = await this.PrepareCartWithOrdersViewModelByUserIdAsync(userId);

        return View(viewModel);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddToCart([FromRoute] Guid id, [FromForm] int quantity)
    {
        if (id == Guid.Empty || quantity < 1)
            return BadRequest();
        
        try
        {
            Guid userId = this.GetUserId(throwIfNull: true);

            await this._cartsService.AddProductToCartAsync(userId, id, quantity);

            return RedirectToAction(nameof(Index), controllerName: "Cart");
        }
        catch (InsufficientProductQuantityInStockException notInStockEx)
        {
            this._logger.LogWarning(
                notInStockEx,
                string.Format(DefaultLogExceptionMessageWithControllerAndAction,
                    nameof(InsufficientProductQuantityInStockException),
                    this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName));
            
            TempData["CartModificationErrorMessage"] = CartModificationErrorMessage;
            return RedirectToAction("Details", controllerName: "Products", new { id });
        }
        catch (ProductDisabledException prodStatusEx)
        {
            this._logger.LogWarning(
                prodStatusEx,
                string.Format(DefaultLogExceptionMessageWithControllerAndAction,
                    nameof(ProductDisabledException),
                    this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName));
            
            TempData["CartModificationErrorMessage"] = CartModificationErrorMessage;
            return RedirectToAction("Details", controllerName: "Products", new { id });
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveFromCart([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid userId = this.GetUserId(throwIfNull: true);
        await this._cartsService.RemoveProductFromCartAsync(userId, id);

        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SubmitOrder()
    {
        Guid userId = this.GetUserId(throwIfNull: true);

        SubmitOrderResultDto res = await this._ordersService.SubmitOrderAsync(userId);
        if (!res.IsSuccess)
        {
            if (res.ErrorProducts.Any())
                TempData["ProblemWithCartProductMessage"] = ProblemWithCartProductMessage;
            else
                TempData["CartModificationErrorMessage"] = CartModificationErrorMessage;
        }
        else
        {
            TempData["OrderSubmittionSuccessMessage"] = OrderSubmittionSuccessMessage;
        }
        
        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cancel()
    {
        Guid userId = this.GetUserId(throwIfNull: true);
        await this._cartsService.DeleteCart(userId);

        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }
    
    [Authorize]
    [SkipStatusCodePages]
    [AcceptVerbs("POST")]
    public async Task<IActionResult> VerifyProdQty(
        [FromBody] ValidateProductQtyInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (inputModel.Id == Guid.Empty)
            throw new ArgumentException($"Id can not be empty.", nameof(inputModel.Id));

        try
        {
            Guid userId = this.GetUserId(throwIfNull: true);

            ProductQtyValidationDto dto = new ProductQtyValidationDto()
            {
                Id = inputModel.Id,
                QuantityRequested = inputModel.Quantity,
            };

            bool isValidProdQtyToAdd = await this._cartsService
                .IsValidProductQtyToAddToCartAsync(userId, dto);

            return Ok(isValidProdQtyToAdd);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, string.Format(RemoteValidationErrorMessage,
                this.GetType().Name, ControllerContext.ActionDescriptor.ActionName));

            return BadRequest(ex);
        }
    }

    private async Task<CartWithOrdersViewModel> PrepareCartWithOrdersViewModelByUserIdAsync(
        Guid userId,
        CartDto? userCartDto = null,
        IEnumerable<OrderDto>? userOrdersDtos = null)
    {
        userCartDto ??= await this._cartsService.GetCartByUserIdAsync(userId);
        userOrdersDtos ??= await this._ordersService.GetAllOrdersByUserIdAsync(userId);

        CartWithOrdersViewModel viewModel = new CartWithOrdersViewModel()
        {
            CartViewModel = userCartDto == null
                ? null
                : new CartViewModel()
                {
                    CartId = userCartDto.CartId,
                    TotalPrice = userCartDto.TotalPrice,
                    CartProducts = userCartDto.CartProducts
                        .Select(cp => new CartProductViewModel()
                        {
                            Id = cp.Id,
                            Name = cp.Name,
                            QuantityAdded = cp.QuantityAdded,
                            UnitPrice = cp.UnitPrice,
                            TotalPrice = cp.TotalPrice,
                            IsEnabled = cp.IsEnabled,
                            IsEnoughQtyLeft = cp.IsEnoughQtyLeft
                        }),
                },
            OrderViewModels = userOrdersDtos
                .Select(o => new OrderViewModel()
                {
                    Id = o.Id,
                    SubmittedOn = o.SubmittedOn,
                    OrderProducts = o.OrderProducts
                        .Select(op => new OrderProductViewModel()
                        {
                            Id = op.Id,
                            Name = op.Name,
                            QuantityOrdered = op.QuantityOrdered,
                            TotalPrice = op.TotalPriceAtOrderTime,
                            UnitPrice = op.UnitPriceAtOrderTime
                        }),
                    TotalPrice = o.TotalPrice,
                }),
        };

        return viewModel;
    }
}