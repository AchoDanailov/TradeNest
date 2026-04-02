using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeNest.Web.Utilities.Exceptions;

namespace TradeNest.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[AutoValidateAntiforgeryToken]
public abstract class BaseAdminController : Controller
{
    protected Guid GetAdminUserId(bool throwIfNull)
    {
        string? userId = this.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null &&
            Guid.TryParse(userId, out Guid userIdGuidValue) &&
            userIdGuidValue != Guid.Empty)
        {
            return userIdGuidValue;
        }
        
        if(!throwIfNull)
            return Guid.Empty;

        string areaName
            = ControllerContext.RouteData.Values["area"] as string ?? string.Empty;
        string controllerName 
            = ControllerContext.RouteData.Values["controller"] as string ?? string.Empty;
        string actionName 
            = ControllerContext.RouteData.Values["action"] as string ?? string.Empty;
        
        throw new UserIdMissingException(controllerName, actionName, areaName);
    }
}