using Microsoft.AspNetCore.Mvc;
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
        
        string area = ControllerContext.RouteData.Values["area"] as string ?? string.Empty;
        string controller = ControllerContext.RouteData.Values["controller"] as string ?? string.Empty;
        string action = ControllerContext.RouteData.Values["action"] as string ?? string.Empty;
        returnUrl ??= $"{area}/{controller}/{action}";
        
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
    public async Task<IActionResult> ManageAllRoles()
    {
        return Json(StatusCodes.Status204NoContent);
    }
}