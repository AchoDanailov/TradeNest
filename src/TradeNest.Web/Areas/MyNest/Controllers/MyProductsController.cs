using Microsoft.AspNetCore.Mvc;

using TradeNest.Web.ViewModels.Enums;
using TradeNest.Web.ViewModels.MyNest;

namespace TradeNest.Web.Areas.MyNest.Controllers;

public class MyProductsController : BaseMyNestController
{
    public IActionResult Index()
    {
        var model = new SellerDashboardViewModel
        {
            TotalSales = 15,
            TotalRevenue = 1050m,
            TotalSurplus = 650m,
            ApprovedProducts = new List<SellerProductViewModel>
            {
                new SellerProductViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Vintage Film Camera",
                    CategoryName = "Electronics",
                    ImageUrl = "/images/products/camera.jpg",
                    CostPrice = 50.00m,
                    UnitPrice = 120.00m,
                    TimesSold = 5,
                    TotalSurplus = 350.00m,
                    IsEnabled = true,
                    ApprovalStatus = ApprovalStatus.Approved
                },
                new SellerProductViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Handcrafted Leather Wallet",
                    CategoryName = "Accessories",
                    ImageUrl = null,
                    CostPrice = 15.00m,
                    UnitPrice = 45.00m,
                    TimesSold = 10,
                    TotalSurplus = 300.00m,
                    IsEnabled = true,
                    ApprovalStatus = ApprovalStatus.Approved
                }
            },
            NonApprovedProducts = new List<SellerProductViewModel>
            {
                new SellerProductViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Luxury Gold Watch",
                    CategoryName = "Jewelry",
                    ImageUrl = null,
                    CostPrice = 200.00m,
                    UnitPrice = 500.00m,
                    TimesSold = 0,
                    TotalSurplus = 0m,
                    IsEnabled = false,
                    ApprovalStatus = ApprovalStatus.WaitingApproval
                },
                new SellerProductViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Broken Toaster (Parts Only)",
                    CategoryName = "Home Appliances",
                    ImageUrl = null,
                    CostPrice = 5.00m,
                    UnitPrice = 2.00m,
                    TimesSold = 0,
                    TotalSurplus = 0m,
                    IsEnabled = false,
                    ApprovalStatus = ApprovalStatus.Disapproved
                }
            }
        };

        return View(model);
    } 
}