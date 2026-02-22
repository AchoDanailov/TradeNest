using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.Utilities;
using TradeNest.Web.ViewModels.Order;

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
            Guid userId = this.GetUserId();
            IEnumerable<OrderViewModel>userOrders = await this._ordersService
                .GetAllOrdersByUserIdAsync(userId);
            
            return View(userOrders);
        }
        catch (Exception err)
        {
            this._logger.LogCritical(err.Message,
                "An unexpected error occured while user tried to access his Orders. See internal error.");

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
            Guid userId = this.GetUserId();
            await this._ordersService
                .AddProductToOrderAsync(userId, id, quantity);
            
            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying to add a product in the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OperationsStatusMessages.OrderModificationUnexpectedErrorMessage;

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
            Guid userId = this.GetUserId();
            await this._ordersService
                .RemoveProductFromOrderAsync(userId, id, orderId);

            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying to remove a product from the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OperationsStatusMessages.OrderModificationUnexpectedErrorMessage;

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
            Guid userId = this.GetUserId();
            await this._ordersService.SubmitOrderAsync(userId, id);
            
            TempData["OrderSubmittionSuccessMessage"]
                = OperationsStatusMessages.OrderSubmittionSuccessMessage;
            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying to submit the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OperationsStatusMessages.OrderModificationUnexpectedErrorMessage;

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
            Guid userId = this.GetUserId();
            await this._ordersService.CancelOrderAsync(userId, id);
            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying to cancel the user's ongoing order.");
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OperationsStatusMessages.OrderModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }

    [AllowAnonymous]
    [AcceptVerbs("POST")]
    public async Task<IActionResult> VerifyProdQty([FromBody] ValidateProductQtyInputModel inputModel)
    {
        if (inputModel.Id == Guid.Empty)
            return Json(false);
        
        Guid userId = this.GetUserId();
        if (userId == Guid.Empty)
            return Json(true);
        
        bool isValidProdQtyToAdd = await this._ordersService
            .IsValidProductQtyToOrder(userId, inputModel);
        
        return Json(isValidProdQtyToAdd);
    }
}