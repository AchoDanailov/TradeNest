namespace TradeNest.Web.Utilities.Messages;

/// <summary>
/// This class provides with titles and messages for status code page responses from the application to the client.
/// </summary>
internal static class StatusCodesPagesMessages
{
    internal static class NotFound404
    {
        internal const string Title = "Page Not Found";
        internal const string Message 
            = "We couldn’t find the page you are looking for. Please check the link or return to the homepage.";
    }
    
    internal static class BadRequest400
    {
        internal const string Title = "Invalid Request";
        internal const string Message = "The request could not be processed. Please try again.";
    }

    internal static class Forbidden403
    {
        internal const string Title = "You don't have access.";
        internal const string Message = "You don’t have permission to view this page.";
    }
    
    internal static class InternalServerError500
    {
        internal const string Title = "Something went wrong.";
        internal const string Message 
            = "We’re experiencing a temporary issue. Please try again in a few minutes. If the problem persists, contact support.";
    }
}
