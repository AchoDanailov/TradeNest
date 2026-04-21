using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Category;
using TradeNest.GCommon;
using TradeNest.Web.ViewModels.Category;
using static TradeNest.Web.Utilities.Messages.StatusNotificationMessages;

namespace TradeNest.Web.Areas.Admin.Controllers;

public class ProductsManagementController : BaseAdminController
{
    private readonly ICategoriesService _categoriesService;

    public ProductsManagementController(ICategoriesService categoriesService)
    {
        this._categoriesService = categoriesService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ManageCategories()
    {
        IEnumerable<CategoryDto> categoryDtos = await this._categoriesService
            .GetAllCategoriesAsync();
        IEnumerable<AllCategoriesViewModel> categoriesViewModels = categoryDtos
            .Select(c => new AllCategoriesViewModel()
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
            });
        
        return View(categoriesViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> CreateCategory()
    {
        return View(new CreateCategoryFormModel());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryFormModel formModel)
    {
        if (!ModelState.IsValid)
            return View(formModel);

        Guid userId = this.GetAdminUserId(throwIfNull: true);
        await this._categoriesService.CreateCategoryAsync(userId, formModel.CategoryName);

        TempData["SuccessfullyCreatedCategoryMessage"] 
            = string.Format(SuccessfullyCreatedCategoryMessage, formModel.CategoryName);
        return RedirectToAction(nameof(ManageCategories), controllerName: "ProductsManagement");
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveCategory([FromRoute(Name = "id")] Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return BadRequest();

        Guid userId = this.GetAdminUserId(throwIfNull: true);

        string messageField = "UnexpectedErrorMessage";
        string messageValue = UnexpectedErrorMessage;
        
        DeleteCategoryResultDto result = await this._categoriesService
            .DeleteCategoryByIdAsync(userId, categoryId);
        
        if (result.IsSuccess)
        {
            switch (result.WereProductsMoved)
            {
                case true:
                    messageField = "CategoryDeletionSuccessFullMessage";
                    messageValue = string.Format(CategoryDeletionSuccessFullMessage,
                        ApplicationConstants.DefaultProductsCategory);
                    break;
                
                case false:
                    messageField = "CategoryDeletionSuccessMessage";
                    messageValue = CategoryDeletionSuccessMessage;
                    break;
            }
        }
        else
        {
            switch (result.FailureReason)
            {
                case ExpectedFailureReason.NoCategoryToMoveProductsTo:
                    messageField = "NoDefaultCategoryMessage";
                    messageValue = string.Format(NoDefaultCategoryMessage,
                        ApplicationConstants.DefaultProductsCategory);
                    break;
                
                case ExpectedFailureReason.RemovingDefaultCategory:
                    messageField = "RemovingDefaultCategoryMessage";
                    messageValue = string.Format(RemovingDefaultCategoryMessage,
                        ApplicationConstants.DefaultProductsCategory);
                    break;
            }
        }

        TempData[messageField] = messageValue;
        return RedirectToAction(
            actionName: nameof(ManageCategories),
            controllerName: "ProductsManagement",
            routeValues:new { area = "Admin" });
    }
}