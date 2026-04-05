using Microsoft.AspNetCore.Mvc;
using TradeNest.Web.ViewModels.Role;
using TradeNest.Web.ViewModels.User;

namespace TradeNest.Web.Areas.Admin.Controllers;

public class UsersManagementController : BaseAdminController
{
    [HttpGet]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        IEnumerable<ManageUserViewModel> manageUsersViewModels = new List<ManageUserViewModel>()
        {
            new ManageUserViewModel()
            {
                Id = "3",
                Username = "Admin1",
                Email = "Admin1@gmail.com",
                UserRoles = new List<RoleViewModel>()
                {
                    new RoleViewModel()
                    {
                        Id = "0",
                        RoleName = "Admin",
                    }
                }
            },
            new ManageUserViewModel()
            {
                Id = "1",
                Username = "Pesho",
                Email = "Pesho@gmail.com",
                UserRoles = new List<RoleViewModel>()
                {
                    new RoleViewModel()
                    {
                        Id = "1",
                        RoleName = "User",
                    }
                }
            },
            new ManageUserViewModel()
            {
                Id = "2",
                Username = "Minchu",
                Email = "Minchu@gmail.com",
                UserRoles = new List<RoleViewModel>()
                {
                    new RoleViewModel()
                    {
                        Id = "1",
                        RoleName = "User",
                    }
                }
            }
        };

        returnUrl ??= Url.Action(nameof(Index), controller: "Home", new { area = "Admin"});
        
        return View(new ManageAllUsersViewModel()
        {
            Users = manageUsersViewModels,
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
            ReturnUrl = returnUrl,
        });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveUser()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesFormModel formModel)
    {
        return Json(new 
        {
            formModel.Id,
            formModel.Username,
            Roles = formModel.AllRoles.Select(r => new
            {
                r.Id,
                r.RoleName,
                r.IsAssigned,
                r.IsActionTaken,
            })
        });
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
    public async Task<IActionResult> RemoveRole(Guid id, string? returnUrl = null)
    {
        // dont forget to validate for guid.empty since the model binder binds guid.empty on invalid guids.
        return Json(new { res = "works" });
    }
}