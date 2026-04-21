using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Product;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.Models.Enums;
using TradeNest.Web.Models.MyNest;

namespace TradeNest.Web.Areas.MyNest.Controllers;

public class MyProductsController : BaseMyNestController
{
    private readonly IProductsService _productsService;
    private readonly IProductPresentationModelsMapper _productsMapper;

    public MyProductsController(IProductsService productsService, 
        IProductPresentationModelsMapper productsMapper)
    {
        this._productsService = productsService;
        this._productsMapper = productsMapper;
    }

    public async Task<IActionResult> Index()
    {
        Guid userId = this.GetUserId(throwIfNull: true);
        
        SellerProductsStatsDto sellerProductsStats = await this._productsService
            .GetSellerProductsStatisticsAsync(userId);
        
        IEnumerable<SellerProductDto> sellerProductDtos = await this._productsService
            .GetSellerProductsAsync(userId);
        IEnumerable<SellerProductViewModel> sellerProductViewModels = this._productsMapper
            .ToSellerProductViewModels(sellerProductDtos)
            .ToArray();
        
        SellerDashboardViewModel model = new SellerDashboardViewModel
        {
            TotalSales = sellerProductsStats.TotalSales,
            TotalRevenue = sellerProductsStats.TotalRevenue,
            TotalSurplus = sellerProductsStats.TotalSurplus,
            
            HasProductsWithoutCostPrice = sellerProductViewModels
                .Any(p => p.CostPrice == null),
            
            ApprovedProducts = sellerProductViewModels
                .Where(p => p.ApprovalStatus == ApprovalStatus.Approved),
            
            NonApprovedProducts = sellerProductViewModels
                .Where(p => p.ApprovalStatus != ApprovalStatus.Approved)
        };

        return View(model);
    } 
}