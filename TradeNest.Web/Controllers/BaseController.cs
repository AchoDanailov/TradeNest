using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Controllers;

/// <summary>
/// Provides with common utilities for all controllers and ensures compliance with no authority by default rule.
/// </summary>
[Authorize]
public abstract class BaseController : Controller
{
    protected Guid GetUserId()
    {
        string? userId = this.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Guid.Parse(userId);
    }
}