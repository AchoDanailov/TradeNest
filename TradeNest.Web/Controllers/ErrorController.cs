using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TradeNest.GCommon.Exceptions;
using TradeNest.Web.Utilities;

namespace TradeNest.Web.Controllers;

[AllowAnonymous]
public class ErrorController : BaseController
{
    private ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        this._logger = logger;
    }

    public IActionResult Index()
    {
        IExceptionHandlerPathFeature? originalRequest 
            = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (originalRequest == null)
        {
            this._logger.LogError(ErrorMessages.ExceptionHandlerUnexpectedException);
            return View("InternalError");
        }

        ViewResult viewToReturn = this.MapExceptionToView(originalRequest, 
            out string logMessage, out LogLevel logLevel);
        
        this._logger.Log(logLevel, originalRequest.Error, logMessage);
        return viewToReturn;
    }
    
    public IActionResult StatusCode(string statusCode)
    {
        throw new NotImplementedException();
    }

    private ViewResult MapExceptionToView(IExceptionHandlerPathFeature originalRequest,
        out string logMessage, out LogLevel logLevel)
    {
        logMessage = this.BuildLogMessage(originalRequest);
        
        Exception thrownException = originalRequest.Error;
        if (thrownException is InsufficientProductQuantityInStockException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        }
        else if (thrownException is ResourceNotFoundException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else if (thrownException is UnauthorizedOperationException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else if (thrownException is ArgumentException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else if (thrownException is InvalidOperationException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logLevel = LogLevel.Error;
            return View("ServerError");
        }

        logLevel = LogLevel.Warning;
        return View("ClientError");
    }

    private string BuildLogMessage(IExceptionHandlerPathFeature originalRequest)
    {
        StringBuilder logMessage = new StringBuilder();

        logMessage.AppendFormat(
            ErrorMessages.DefaultLogExceptionMessage,
            originalRequest.Error.GetType().Name);

        if (originalRequest.RouteValues?
                .TryGetValue("controller", out object? controllerName) is true)
        {
            logMessage.AppendFormat(" Controller: {0}",
                controllerName as string ?? string.Empty);
        }

        if (originalRequest.RouteValues?
                .TryGetValue("action", out object? actionName) is true)
        {
            logMessage.AppendFormat(" Action: {0}",
                actionName as string ?? string.Empty);
        }
                
        return logMessage.ToString();
    }
}