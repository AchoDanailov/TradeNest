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
        // IEnumerable<AllOrdersViewModel> userOrders = Array.Empty<AllOrdersViewModel>();
        //
        // try
        // {
        //     Guid userId = this.GetUserId();
        //     userOrders = await this._ordersService.GetAllOrdersByUserIdAsync(userId);
        // }
        // catch (Exception err)
        // {
        //     this._logger.LogWarning(err.Message,
        //         "An unexpected error occured while user tried to access his Orders. See internal error.");   
        // } 
        // return View(userOrders);
        return View(GenerateOrdersWithSingleOngoing());
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Cancel()
    {
        throw new NotImplementedException();
    }
    
    public static IEnumerable<AllOrdersViewModel> GenerateOrdersWithSingleOngoing()
    {
        return new List<AllOrdersViewModel>
        {
            // The single ongoing order (unchanged)
            new AllOrdersViewModel
            {
                Id = Guid.NewGuid(),
                TotalPrice = 120.50m,
                IsSubmitted = false,
                SubmittedOn = null,
                OrderProducts = new List<OrderProductViewModel>
                {
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bluetooth Speaker",
                        QuantityOrdered = 1,
                        UnitPrice = "75.00",
                        TotalPrice = "75.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Gaming Keyboard",
                        QuantityOrdered = 1,
                        UnitPrice = "75.00",
                        TotalPrice = "75.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Gaming Mouse",
                        QuantityOrdered = 1,
                        UnitPrice = "45.50",
                        TotalPrice = "45.50"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "HDMI Cable",
                        QuantityOrdered = 2, // Quantity > 1
                        UnitPrice = "10.00",
                        TotalPrice = "20.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Pen Set",
                        QuantityOrdered = 1,
                        UnitPrice = "20.00",
                        TotalPrice = "20.00"
                    }
                }
            },
            // Submitted Order 1: With more products, including one with QTY > 1
            new AllOrdersViewModel
            {
                Id = Guid.NewGuid(),
                TotalPrice = 620.00m,
                IsSubmitted = true,
                SubmittedOn = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-dd HH:mm"),
                OrderProducts = new List<OrderProductViewModel>
                {
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "4K Monitor",
                        QuantityOrdered = 1,
                        UnitPrice = "500.00",
                        TotalPrice = "500.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "HDMI Cable",
                        QuantityOrdered = 2, // Quantity > 1
                        UnitPrice = "10.00",
                        TotalPrice = "20.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Desk Mat",
                        QuantityOrdered = 1,
                        UnitPrice = "50.00",
                        TotalPrice = "50.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Webcam",
                        QuantityOrdered = 1,
                        UnitPrice = "50.00",
                        TotalPrice = "50.00"
                    }
                }
            },
            // Submitted Order 2: Another order with multiple products
            new AllOrdersViewModel
            {
                Id = Guid.NewGuid(),
                TotalPrice = 105.00m,
                IsSubmitted = true,
                SubmittedOn = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd HH:mm"),
                OrderProducts = new List<OrderProductViewModel>
                {
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bluetooth Speaker",
                        QuantityOrdered = 1,
                        UnitPrice = "75.00",
                        TotalPrice = "75.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Phone Charger",
                        QuantityOrdered = 1,
                        UnitPrice = "30.00",
                        TotalPrice = "30.00"
                    }
                }
            },
            // Existing submitted order (modified slightly to fit more products theme)
            new AllOrdersViewModel
            {
                Id = Guid.NewGuid(),
                TotalPrice = 50.00m,
                IsSubmitted = true,
                SubmittedOn = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd HH:mm"),
                OrderProducts = new List<OrderProductViewModel>
                {
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Notebook",
                        QuantityOrdered = 3, // Quantity > 1
                        UnitPrice = "10.00",
                        TotalPrice = "30.00"
                    },
                    new OrderProductViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Pen Set",
                        QuantityOrdered = 1,
                        UnitPrice = "20.00",
                        TotalPrice = "20.00"
                    }
                }
            }
        };
    }
}