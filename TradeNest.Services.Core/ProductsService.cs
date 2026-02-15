using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.GCommon;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Services.Core;

public class ProductsService : IProductsService
{
    private readonly TradeNestDbContext _dbContext;

    public ProductsService(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }
    
    public CatalogProductsAndCategoriesViewModel CreateEmptyCatalogProdsAndCategoriesDto()
    {
        CatalogProductsAndCategoriesViewModel viewModel 
            = new CatalogProductsAndCategoriesViewModel();
        
        return viewModel;
    }

    public async Task<CatalogProductsAndCategoriesViewModel> GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
        IEnumerable<ProductViewModel>? productsViewModels = null)
    {
        CatalogProductsAndCategoriesViewModel viewModel
            = this.CreateEmptyCatalogProdsAndCategoriesDto();
        
        viewModel.Categories = await this.GetAllCategoriesViewModels();

        if (productsViewModels != null)
            viewModel.Products = productsViewModels;

        return viewModel;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProductVmsOrderedByCreatedOnAsync()
    {
        IEnumerable<ProductViewModel> productsViewModels = await this._dbContext.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(ApplicationConstants.PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            })
            .ToArrayAsync();

        return productsViewModels;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsWithSearchQueryForNameAsync(
        string searchQuery)
    {
        if (string.IsNullOrEmpty(searchQuery))
            return Array.Empty<ProductViewModel>();
        
        return await this._dbContext.Products
            .AsNoTracking()
            .Where(p => p.Name.Contains(searchQuery))
            .OrderByDescending(p => p.CreatedOn)
            .ThenBy(p => p.Name)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(ApplicationConstants.PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            }) 
            .ToArrayAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return Array.Empty<ProductViewModel>();
        
        return await this._dbContext.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedOn)
            .ThenBy(p => p.Name)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(ApplicationConstants.PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            }) 
            .ToArrayAsync();
    }

    private async Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModels()
    {
        return await this._dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new AllCategoriesViewModel()
            {
                Id = c.Id,
                CategoryName = c.Name,
            })
            .ToArrayAsync();
    }
}