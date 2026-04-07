using Microsoft.AspNetCore.Mvc;

using static TradeNest.Web.Utilities.Messages.StatusNotificationMessages;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Role;
using TradeNest.Services.Models.User;
using TradeNest.Web.Mappers.Interfaces;
using TradeNest.Web.ViewModels.Role;
using TradeNest.Web.ViewModels.User;

namespace TradeNest.Web.Areas.Admin.Controllers;

public class UsersManagementController : BaseAdminController
{
    private readonly IUsersService _usersService;
    private readonly IUsersPresentationModelsMapper _usersMapper;

    public UsersManagementController(IUsersService usersService,
        IUsersPresentationModelsMapper usersMapper)
    {
        this._usersService = usersService;
        this._usersMapper = usersMapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        if (returnUrl == null || !Url.IsLocalUrl(returnUrl))
            returnUrl = Url.Action(nameof(Index), controller: "Home", new { area = "Admin"});

        Guid userId = this.GetAdminUserId(throwIfNull: true);
        
        IEnumerable<UserDto> usersDtos = await this._usersService.GetAllUsersAsync(userId);
        IEnumerable<RoleDto> allRoles = await this._usersService.GetAllRolesAsync(userId);

        IEnumerable<ManageUserViewModel> usersViewModels = this._usersMapper
            .ToManageUserViewModels(usersDtos);
        IEnumerable<RoleViewModel> rolesViewModels = this._usersMapper
            .ToRoleViewModels(allRoles);

        ManageAllUsersViewModel viewModel = new ManageAllUsersViewModel()
        {
            Users = usersViewModels,
            AllRoles = rolesViewModels,
            ReturnUrl = returnUrl
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveUser([FromRoute(Name = "id")] Guid userToDeleteId)
    {
        if (userToDeleteId == Guid.Empty)
            return BadRequest();
        
        Guid adminUserId = this.GetAdminUserId(throwIfNull: true);
        await this._usersService.DeleteUserByIdAsync(adminUserId, userToDeleteId);

        TempData["SuccessfullyRemovedUserMessage"] = SuccessfullyRemovedUserMessage;
        return RedirectToAction(
            actionName: nameof(Index),
            controllerName: "UsersManagement",
            routeValues: new { area = "Admin" });
    }

    [HttpPost]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesFormModel formModel)
    {
        if (!ModelState.IsValid || formModel.Id == Guid.Empty ||
            formModel.AllRoles.Any(r => r.Id == Guid.Empty))
        {
            return BadRequest();
        }

        Guid adminUserId = this.GetAdminUserId(throwIfNull: true);
        
        ModifyUserRolesDto modifyUserRolesDto = this._usersMapper
            .FromManageUserFormModel(formModel);

        await this._usersService.ModifyUserRoles(adminUserId, modifyUserRolesDto);
        return RedirectToAction(
            actionName: nameof(Index),
            controllerName: "UsersManagement",
            routeValues: new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> ManageAllRoles(string? returnUrl = null)
    {
        return View(new ManageAllRolesViewModel()
        {
            AllRoles = new List<RoleViewModel>()
            {
                new RoleViewModel()
                {
                    Id = "0",
                    RoleName = "Admin",
                },
                new RoleViewModel()
                {
                    Id = "1",
                    RoleName = "User",
                },
            },
            ReturnUrl = returnUrl ?? 
                        Url.Action(nameof(Index), controller: "UsersManagement")
        });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole(Guid userToDeleteId, string? returnUrl = null)
    {
        // dont forget to validate for guid.empty since the model binder binds guid.empty on invalid guids.
        return Json(new { res = "works" });
    }
}