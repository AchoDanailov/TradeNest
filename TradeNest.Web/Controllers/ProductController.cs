using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.Web.ViewModels;

namespace TradeNest.Web.Controllers;

public class ProductController : Controller
{
    private TradeNestDbContext _dbContext;

    public ProductController(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Details([FromRoute] string? id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guidId))
        {
            return BadRequest();
        }

        var product = this._dbContext.Products
            .Include(p => p.Owner)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == guidId);
        if (product == null)
        {
            return NotFound();
        }

        bool isOwner = IsOwner();
        
        ProductDetailsViewModel productDetailsViewModel = new ProductDetailsViewModel()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            QuantityInStock = product.QuantityInStock,
            SellingPrice = product.SellingPrice.ToString("f2"),
            IsEnabled = product.IsEnabled,
            Owner = product.Owner!.UserName ?? string.Empty,
            CategoryName = product.Category.Name,
            FrontImageUrl = product.Images
                .FirstOrDefault(i => i.IsFrontImage == true)!
                .Url,
            ImagesUrls = product.Images
                .Select(i => i.Url),
            IsOwner = isOwner
        };
        
        return View(productDetailsViewModel);
    }

    //TODO: Implement
    private bool IsOwner()
    {
        return false;
    }
}