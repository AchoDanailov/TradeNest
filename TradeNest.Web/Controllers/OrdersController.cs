using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Order;
using TradeNest.Web.Utilities.Exceptions;
using static TradeNest.Web.Utilities.Messages.StatusNotificationMessages;
using static TradeNest.Web.Utilities.Messages.LoggingErrorMessages;

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
        Guid? userId = this.GetUserId();
        if (userId == null || userId.Value == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }
                
        IEnumerable<OrderViewModel> userOrders = await this._ordersService
            .GetAllOrdersByUserIdAsync(userId.Value);
            
        return View(userOrders);
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
            {
                throw new UserIdMissingException(this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName);
            }

            await this._ordersService
                .AddProductToOrderAsync(userId.Value, id, quantity);

            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, string.Format(BadArgumentsErrorMessage,
                this.GetType().Name, ControllerContext.ActionDescriptor.ActionName));
            
            TempData["OrderModificationUnexpectedErrorMessage"]
                = OrderModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveProduct([FromRoute] Guid id, [FromForm] Guid orderId)
    {
        if (id == Guid.Empty || orderId == Guid.Empty)
            return BadRequest();

        Guid? userId = this.GetUserId();
        if (userId == null || userId == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }

        await this._ordersService
            .RemoveProductFromOrderAsync(userId.Value, id, orderId);

        return RedirectToAction(nameof(Index), controllerName: "Orders");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Submit([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid? userId = this.GetUserId();
        if (userId == null || userId == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }
            
        await this._ordersService.SubmitOrderAsync(userId.Value, id);
            
        TempData["OrderSubmittionSuccessMessage"] = OrderSubmittionSuccessMessage;
        return RedirectToAction(nameof(Index), controllerName: "Orders");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cancel([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid? userId = this.GetUserId();
        if (userId == null || userId == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }

        await this._ordersService.CancelOrderAsync(userId.Value, id);
            
        return RedirectToAction(nameof(Index), controllerName: "Orders");
    }
    
    [Authorize]
    [SkipStatusCodePages]
    [AcceptVerbs("POST")]
    public async Task<IActionResult> VerifyProdQty(
        [FromBody] ValidateProductQtyInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(new Exception());

        if (inputModel.Id == Guid.Empty)
        {
            ModelState.AddModelError(nameof(inputModel.Id), "Id can not be empty.");
            return BadRequest(ModelState);
        }

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
            {
                throw new UserIdMissingException(this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName);
            }

            bool isValidProdQtyToAdd = await this._ordersService
                .IsValidProductQtyToOrderAsync(userId.Value, inputModel);

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