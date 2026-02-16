using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeNest.Services.Core.Interfaces;
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
        IEnumerable<AllOrdersViewModel> userOrders = Array.Empty<AllOrdersViewModel>();

        try
        {
            Guid userId = this.GetUserId();
            userOrders = await this._ordersService.GetAllOrdersByUserIdAsync(userId);
        }
        catch (Exception err)
        {
            this._logger.LogWarning(err.Message,
                "An unexpected error occured while user tried to access his Orders. See internal error.");   
        }
        
        return View(userOrders);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> OnGoing()
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProduct([FromRoute] string productId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveProduct([FromRoute] string productId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Submit([FromRoute] string productId)
    {
        throw new NotImplementedException();
    }
}