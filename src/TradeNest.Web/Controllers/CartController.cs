using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Cart;
using TradeNest.Services.Models.Order;
using TradeNest.Services.Models.Product;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.ViewModels.Order;
using TradeNest.Web.ViewModels.Cart;
using TradeNest.Web.ViewModels.Product;
using static TradeNest.Web.Utilities.Messages.StatusNotificationMessages;

namespace TradeNest.Web.Controllers;

public class CartController : BaseController
{
    private readonly ILogger<CartController> _logger;
    private readonly IOrdersService _ordersService;
    private readonly ICartsService _cartsService;
    private readonly IOrderPresentationModelsMapper _orderPresentationModelsMapper;
    private readonly ICartPresentationModelsMapper _cartPresentationModelsMapper;

    public CartController(
        IOrdersService ordersService,
        ICartsService cartsService,
        ILogger<CartController> logger, 
        ICartPresentationModelsMapper cartPresentationModelsMapper,
        IOrderPresentationModelsMapper orderPresentationModelsMapper)
    {
        this._logger = logger;
        this._ordersService = ordersService;
        this._cartsService = cartsService;
        this._cartPresentationModelsMapper = cartPresentationModelsMapper;
        this._orderPresentationModelsMapper = orderPresentationModelsMapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Guid userId = this.GetUserId(throwIfNull: true);
        
        CartDto? userCartDto = await this._cartsService
            .GetCartByUserIdAsync(userId);
        IEnumerable<OrderDto> userOrdersDtos = await this._ordersService
            .GetAllOrdersByUserIdAsync(userId);

        CartViewModel? cartViewModel = userCartDto != null
            ? this._cartPresentationModelsMapper.ToCartViewModel(userCartDto)
            : null;
        IEnumerable<OrderViewModel> orderViewModels = this._orderPresentationModelsMapper
            .ToOrderViewModels(userOrdersDtos);
        
        CartWithOrdersViewModel viewModel = new CartWithOrdersViewModel()
        {
            CartViewModel = cartViewModel,
            OrderViewModels = orderViewModels,
        };
        
        return View(viewModel);
    }
    
    [HttpPost]
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
            return RedirectToAction("Details", controllerName: "Products", new { id = id });
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
            return RedirectToAction("Details", controllerName: "Products", new { id = id });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid userId = this.GetUserId(throwIfNull: true);
        await this._cartsService.RemoveProductFromCartAsync(userId, id);
        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }
    
    [HttpPost]
    public async Task<IActionResult> SubmitOrder()
    {
        Guid userId = this.GetUserId(throwIfNull: true);

        SubmitOrderResultDto res = await this._ordersService.SubmitOrderAsync(userId);
        if (!res.IsSuccess)
            TempData["ProblemWithCartProductMessage"] = ProblemWithCartProductMessage;
        else
            TempData["OrderSubmittionSuccessMessage"] = OrderSubmittionSuccessMessage;
        
        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }

    [HttpPost]
    public async Task<IActionResult> Cancel()
    {
        Guid userId = this.GetUserId(throwIfNull: true);
        await this._cartsService.DeleteCartAsync(userId);
        return RedirectToAction(nameof(Index), controllerName: "Cart");
    }
    
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
}