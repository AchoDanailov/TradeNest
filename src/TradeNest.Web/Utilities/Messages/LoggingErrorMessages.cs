namespace TradeNest.Web.Utilities.Messages;

/// <summary>
/// Provides with error messages used for logging application errors.
/// </summary>
internal static class LoggingErrorMessages
{
    internal const string ExceptionHandlerUnexpectedException
        = "An unexpected exception occurred in the Error handling controller. Please check if Exception Handling is implemented correctly.";

    internal const string DefaultLogExceptionMessage = "An {0} occurred.";
    
    internal const string DefaultLogExceptionMessageWithControllerAndAction 
        = "An {0} occurred. Controller: {1}, Action: {2}";

    internal const string UnexpectedStatusCodesPagesExceptionMessage
        = "An unexpected exception occurred while using \"UseStatusCodePagesWithReExecute()\" method. Please review the validity of the path passed in to the method and the validity of the method usage.";

    internal const string BadArgumentsErrorMessage
        = "Bad arguments provided. Controller: {0}, Action: {1}";

    internal const string RemoteValidationErrorMessage
        = "Remote validation exception occurred. Controller: {0}, Action: {1}.";
}