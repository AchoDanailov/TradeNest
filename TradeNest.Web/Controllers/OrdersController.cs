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
    private readonly IProductsService _productsService;

    public OrdersController(IOrdersService ordersService, ILogger<OrdersController> logger, IProductsService productsService)
    {
        this._logger = logger;
        this._productsService = productsService;
        this._ordersService = ordersService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        IEnumerable<OrderViewModel> userOrders = Array.Empty<OrderViewModel>();
        
        try
        {
            Guid userId = this.GetUserId();
            userOrders = await this._ordersService.GetAllOrdersByUserIdAsync(userId);
        }
        catch (Exception err)
        {
            this._logger.LogCritical(err.Message,
                "An unexpected error occured while user tried to access his Orders. See internal error.");

            return BadRequest();
        } 
        
        return View(userOrders);
    }
    
    //TODO: If left enough time try adding concurrency stamp to the product's table
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProduct([FromRoute] string id,
        [FromForm] string quantity, string? returnUrl)
    {
        if (string.IsNullOrEmpty(id) ||
            !Guid.TryParse(id, out Guid prodIdGuid) ||
            !int.TryParse(quantity, out int qtyIntValue))
        {
            return BadRequest();
        }

        bool productExists = await this._productsService
            .ProductExistsByIdAsync(prodIdGuid);
        if (!productExists)
            return NotFound();
        
        returnUrl ??= Url.Action(nameof(Index), controller: "Products");
        
        try
        {
            Guid userId = this.GetUserId();
            await this._ordersService
                .AddProductToOrderAsync(userId, prodIdGuid, qtyIntValue);
            
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
    public async Task<IActionResult> RemoveProduct([FromRoute] string id,
        [FromForm] string? returnUrl)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid prodIdGuid))
            return BadRequest();

        bool prodExists = await this._productsService.ProductExistsByIdAsync(prodIdGuid);
        if (!prodExists)
            return NotFound();

        returnUrl ??= Url.Action(nameof(Index), controller: "Orders");

        try
        {
            Guid userId = this.GetUserId();
            await this._ordersService.RemoveProductFromOrderAsync(userId, prodIdGuid);

            return RedirectToAction(nameof(Index), controllerName: "Orders");
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying to remove a product from the user's ongoing order.");
            TempData["ProductRemovingFromOrderUnexpectedErrorMessage"]
                = OperationsStatusMessages.OrderModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Submit([FromRoute] string productId)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Cancel()
    {
        throw new NotImplementedException();
    }
}