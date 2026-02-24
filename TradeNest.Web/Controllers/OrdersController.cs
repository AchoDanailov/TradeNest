using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Order;
using static TradeNest.Web.Utilities.StatusNotificationMessages;

namespace TradeNest.Web.Controllers;

[Authorize]
public class OrdersController : BaseController
{
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrdersService _ordersService;

    public OrdersController(IOrdersService ordersService, ILogger<OrdersController> logger)
    {
        this._logger = logger;
        this._ordersService = ordersService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
            {
                this._logger.LogError("UserId can not be null or empty.");
                TempData["UnexpectedErrorMessage"] = UnexpectedErrorMessage;

                return RedirectToAction(nameof(Index), controllerName: "Home");
            }
                
            IEnumerable<OrderViewModel> userOrders = await this._ordersService
                .GetAllOrdersByUserIdAsync(userId.Value);
            
            return View(userOrders);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while user tried to access his Orders.");

            return BadRequest();
        } 
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProduct([FromRoute] Guid id,
        [FromForm] int quantity, [FromForm] string? returnUrl)
    {
        if (id == Guid.Empty || quantity < 1)
            return BadRequest();

        returnUrl ??= Url.Action("Details", controller: "Products", new { id });
        
        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");

            await this._ordersService
                .AddProductToOrderAsync(userId.Value, id, quantity);

            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (ResourceNotFoundException notFoundEx)
        {
            this._logger.LogWarning(notFoundEx, "Product not found.");
            return NotFound();
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx,
                "Bad arguments were provided while attempting to add product to order.");

            return BadRequest();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while trying to add a product in the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OrderModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveProduct(
        [FromRoute] Guid id,
        [FromForm] Guid orderId,
        [FromForm] string? returnUrl)
    {
        if (id == Guid.Empty || orderId == Guid.Empty)
            return BadRequest();

        returnUrl ??= Url.Action(nameof(Index), controller: "Orders");

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");

            await this._ordersService
                .RemoveProductFromOrderAsync(userId.Value, id, orderId);

            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (ResourceNotFoundException notFoundEx)
        {
            this._logger.LogError(notFoundEx, "Resource not found.");
            return NotFound();
        }
        catch (UnauthorizedOperationException unAuthEx)
        {
            this._logger.LogWarning(unAuthEx, "Unauthorized operation attempt.");
            return Unauthorized();
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, 
                "Bad arguments provided to the remove product from order operation.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while trying to remove a product from the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OrderModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Submit([FromRoute] Guid id, [FromForm] string? returnUrl)
    {
        if (id == Guid.Empty)
            return BadRequest();
        
        returnUrl ??= Url.Action(nameof(Index), controller: "Orders");

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");
            
            await this._ordersService.SubmitOrderAsync(userId.Value, id);
            
            TempData["OrderSubmittionSuccessMessage"] = OrderSubmittionSuccessMessage;
            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (ResourceNotFoundException notFoundEx)
        {
            this._logger.LogWarning(notFoundEx, "Order was not found.");
            return NotFound();
        }
        catch (UnauthorizedOperationException unAuthEx)
        {
            this._logger.LogWarning(unAuthEx, "Unauthorized operation attempted.");
            return Unauthorized();
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, 
                "Bad arguments provided to the submit order operation.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while trying to submit the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OrderModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromForm] string? returnUrl)
    {
        if (id == Guid.Empty)
            return BadRequest();

        returnUrl ??= Url.Action(nameof(Index), controller: "Orders");

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");

            await this._ordersService.CancelOrderAsync(userId.Value, id);
            
            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (ResourceNotFoundException notFoundEx)
        {
            this._logger.LogWarning(notFoundEx, "Order not found.");
            return NotFound();
        }
        catch (UnauthorizedOperationException unAuthEx)
        {
            this._logger.LogWarning(unAuthEx, "Unauthorized operation attempt on order");
            return Unauthorized();
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, 
                "Bad arguments provided while user tried to cancel his order.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while trying to cancel the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OrderModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }

    [Authorize]
    [AcceptVerbs("POST")]
    public async Task<IActionResult> VerifyProdQty(
        [FromBody] ValidateProductQtyInputModel inputModel)
    {
        if (inputModel.Id == Guid.Empty)
            return BadRequest();
        
        inputModel.ReturnUrl ??= Url.Action("Details", controller: "Products",
            new { id = inputModel.Id });

        if (!ModelState.IsValid)
        {
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OrderModificationUnexpectedErrorMessage;
            return LocalRedirect(inputModel.ReturnUrl!);
        }
        
        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");

            bool isValidProdQtyToAdd = await this._ordersService
                .IsValidProductQtyToOrderAsync(userId.Value, inputModel);

            return Json(isValidProdQtyToAdd);
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, "Bad arguments provided.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while user tried to add a product to his order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OrderModificationUnexpectedErrorMessage;
            
            return LocalRedirect(inputModel.ReturnUrl!);
        }
    }
}