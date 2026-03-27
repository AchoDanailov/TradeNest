using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.LoggingErrorMessages;

namespace TradeNest.Web.Infrastructure.Filters;

public class WebApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<WebApiExceptionFilter> _logger;

    public WebApiExceptionFilter(ILogger<WebApiExceptionFilter> logger)
    {
        this._logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        int statusCode;
        string error; 
        
        if (context.Exception is ResourceNotFoundException)
        {
            statusCode = (int)HttpStatusCode.NotFound;
            error = "Not Found";
        }
        else if (context.Exception is UnauthorizedOperationException)
        {
            statusCode = (int)HttpStatusCode.Forbidden;
            error = "Forbidden";
        }
        else if (context.Exception is ArgumentException or InvalidOperationException)
        {
            statusCode = (int)HttpStatusCode.BadRequest;
            error = "Bad Request";
        }
        else
        {
            this._logger.LogError(context.Exception,
                string.Format(
                    DefaultLogExceptionMessageWithControllerAndAction,
                    context.Exception.GetType().Name,
                    string.Empty,
                    context.ActionDescriptor.DisplayName));
            
            statusCode = (int)HttpStatusCode.InternalServerError;
            error = "Internal Server Error";
        }
        
        var errorResponse = new
        {
            Status = statusCode, 
            Error = error,       
        };
        
        context.Result = new JsonResult(errorResponse) { StatusCode = statusCode };
        
        context.ExceptionHandled = true;
    }
}