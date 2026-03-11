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
using TradeNest.Web.Utilities.Exceptions;
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
        Guid? userId = this.GetUserId();
        if (userId == null || userId.Value == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }

        CartWithOrdersViewModel viewModel 
            = await this.PrepareCartWithOrdersViewModel(userId.Value);

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
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
            {
                throw new UserIdMissingException(this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName);
            }

            await this._cartsService.AddProductToCartAsync(userId.Value, id, quantity);

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

        Guid? userId = this.GetUserId();
        if (userId == null || userId == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }

        await this._cartsService.RemoveProductFromCartAsync(userId.Value, id);
        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SubmitOrder()
    {
        Guid? userId = this.GetUserId();
        if (userId == null || userId == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }

        SubmitOrderResultDto res = await this._ordersService.SubmitOrderAsync(userId.Value);
        if (!res.IsSuccess)
        {
            CartWithOrdersViewModel viewModel 
                = await this.PrepareCartWithOrdersViewModel(userId.Value, res.ErrorProducts);
            
            TempData["CartModificationErrorMessage"] = CartModificationErrorMessage;
            return View(nameof(Index), viewModel);
        }

        TempData["OrderSubmittionSuccessMessage"] = OrderSubmittionSuccessMessage;
        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cancel()
    {
        Guid? userId = this.GetUserId();
        if (userId == null || userId == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }

        await this._cartsService.DeleteCart(userId.Value);
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
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
            {
                throw new UserIdMissingException(this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName);
            }

            ProductQtyValidationDto dto = new ProductQtyValidationDto()
            {
                Id = inputModel.Id,
                QuantityRequested = inputModel.Quantity,
            };

            bool isValidProdQtyToAdd = await this._cartsService
                .IsValidProductQtyToAddToCartAsync(userId.Value, dto);

            return Ok(isValidProdQtyToAdd);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, string.Format(RemoteValidationErrorMessage,
                this.GetType().Name, ControllerContext.ActionDescriptor.ActionName));

            return BadRequest(ex);
        }
    }

    private async Task<CartWithOrdersViewModel> PrepareCartWithOrdersViewModel(
        Guid userId,
        IEnumerable<ErrorProductDto>? errorProducts = null)
    {
        CartDto? userCartDto = await this._cartsService.GetCartByUserIdAsync(userId);
        IEnumerable<OrderDto> userOrdersDtos = await this._ordersService
            .GetAllOrdersByUserIdAsync(userId);

        CartWithOrdersViewModel viewModel = new CartWithOrdersViewModel()
        {
            ErrorProductsViewModels = errorProducts != null 
                ? errorProducts
                    .Select(p => new ErrorProductViewModel()
                    {
                        ProductErrorReasons = p.ProductErrorReasons,
                        Id = p.ProductId,
                        ProductName = p.ProductName
                    })
                : new List<ErrorProductViewModel>(),
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
                            TotalPrice = cp.TotalPrice
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