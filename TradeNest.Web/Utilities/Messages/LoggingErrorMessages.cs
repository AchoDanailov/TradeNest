namespace TradeNest.Web.Utilities.Messages;

/// <summary>
/// Provides with error messages used for logging application errors.
/// </summary>
internal static class LoggingErrorMessages
{
    internal const string ExceptionHandlerUnexpectedException
        = "An unexpected exception occured in the Error handling controller. Please check if Exception Handling is implemented correctly.";

    internal const string DefaultLogExceptionMessage = "An {0} occured.";

    internal const string UnexpectedStatusCodesPagesExceptionMessage
        = "An unexpected exception occured while using \"UseStatusCodePagesWithReExecute()\" method. Please review the validity of the path passed in to the method and the validity of the method ussage.";
}