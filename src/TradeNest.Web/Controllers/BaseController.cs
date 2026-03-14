using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TradeNest.Web.Utilities.Exceptions;

namespace TradeNest.Web.Controllers;

/// <summary>
/// Provides with common utilities for all controllers and ensures compliance with no authority by default rule.
/// </summary>
[Authorize]
public abstract class BaseController : Controller
{
    protected Guid GetUserId(bool throwIfNull)
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
        
        string controllerName 
            = ControllerContext.RouteData.Values["controller"] as string ?? string.Empty;
        string actionName 
            = ControllerContext.RouteData.Values["action"] as string ?? string.Empty;
        
        throw new UserIdMissingException(controllerName, actionName);
    }
}