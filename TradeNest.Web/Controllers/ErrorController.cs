using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using TradeNest.GCommon.Exceptions;
using TradeNest.Web.ViewModels.Error;
using TradeNest.Web.Utilities;
using static TradeNest.Web.Utilities.StatusCodesPagesMessages;

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
            this._logger.LogError(LoggingErrorMessages.ExceptionHandlerUnexpectedException);
            return View("ServerError", new ErrorViewModel()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = InternalServerError500.Title,
                Message = InternalServerError500.Message,
            });
        }

        ViewResult viewToReturn = this.HandleException(originalRequest,
            out string logMessage, out LogLevel logLevel);

        this._logger.Log(logLevel, originalRequest.Error, logMessage);
        return viewToReturn;
    }

    public IActionResult StatusCode(string statusCode)
    {
        if (!int.TryParse(statusCode, out int status))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            this._logger.LogError(LoggingErrorMessages.UnexpectedStatusCodesPagesExceptionMessage);
            return View("ServerError", new ErrorViewModel()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = InternalServerError500.Title,
                Message = InternalServerError500.Message,
            });
        }

        ViewResult viewToReturn = this.HandleResponse(status);
        return viewToReturn;
    }

    private ViewResult HandleException(IExceptionHandlerPathFeature originalRequest,
        out string logMessage, out LogLevel logLevel)
    {
        logMessage = this.BuildLogMessage(originalRequest);
        logLevel = LogLevel.Warning;
        ErrorViewModel errorViewModel = new ErrorViewModel();

        Exception thrownException = originalRequest.Error;
        if (thrownException is ResourceNotFoundException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            
            errorViewModel.StatusCode = StatusCodes.Status404NotFound;
            errorViewModel.Title = BadRequest400.Title;
            errorViewModel.Message = BadRequest400.Message;
        }
        else if (thrownException is UnauthorizedOperationException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            
            errorViewModel.StatusCode = StatusCodes.Status403Forbidden;
            errorViewModel.Title = Forbidden403.Title;
            errorViewModel.Message = Forbidden403.Message;
        }
        else if (thrownException is ArgumentException)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            
            errorViewModel.StatusCode = StatusCodes.Status400BadRequest;
            errorViewModel.Title = BadRequest400.Title;
            errorViewModel.Message = BadRequest400.Message;
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            logLevel = LogLevel.Error;
            errorViewModel.StatusCode = StatusCodes.Status500InternalServerError;
            errorViewModel.Title = InternalServerError500.Title;
            errorViewModel.Message = InternalServerError500.Message;
            
            return View("ServerError", errorViewModel);
        }

        return View("ClientError", errorViewModel);
    }

    private string BuildLogMessage(IExceptionHandlerPathFeature originalRequest)
    {
        StringBuilder logMessage = new StringBuilder();

        logMessage.AppendFormat(
            LoggingErrorMessages.DefaultLogExceptionMessage,
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
            logMessage.AppendFormat(", Action: {0}",
                actionName as string ?? string.Empty);
        }

        return logMessage.ToString();
    }

    private ViewResult HandleResponse(int status)
    {
        ErrorViewModel errorViewModel = status switch
        {
            400 => new ErrorViewModel() { StatusCode = StatusCodes.Status400BadRequest, Title = BadRequest400.Title, Message = BadRequest400.Message },
            404 => new ErrorViewModel() { StatusCode = StatusCodes.Status404NotFound, Title = NotFound404.Title, Message = NotFound404.Message },
            _ => new ErrorViewModel() { StatusCode = StatusCodes.Status500InternalServerError, Title = InternalServerError500.Title, Message = InternalServerError500.Message },
        };

        if (status >= 400 && status < 500)
            return View("ClientError", errorViewModel);
        
        return View("ServerError", errorViewModel);
    }
}