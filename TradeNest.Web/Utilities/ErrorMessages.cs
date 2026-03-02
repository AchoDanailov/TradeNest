namespace TradeNest.Web.Utilities;

/// <summary>
/// Provides with error messages used for logging application errors."
/// </summary>
internal static class ErrorMessages
{
    internal const string ExceptionHandlerUnexpectedException
        = "An unexpected exception occured in the Error handling controller. Please check if Exception Handling is implemented correctly.";

    internal const string DefaultLogExceptionMessage = "An {0} occured.";
}