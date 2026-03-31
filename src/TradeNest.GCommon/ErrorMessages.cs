namespace TradeNest.GCommon;

/// <summary>
/// Provides with error messages used for application errors.
/// </summary>
public static class ErrorMessages
{
    public const string ExceptionHandlerUnexpectedException
        = "An unexpected exception occurred in the Error handling controller. Please check if Exception Handling is implemented correctly.";

    public const string DefaultLogExceptionMessage = "An {0} occurred.";
    
    public const string DefaultLogExceptionMessageWithControllerAndAction 
        = "An {0} occurred. Controller: {1}, Action: {2}";

    public const string UnexpectedStatusCodesPagesExceptionMessage
        = "An unexpected exception occurred while using \"UseStatusCodePagesWithReExecute()\" method. Please review the validity of the path passed in to the method and the validity of the method usage.";

    public const string BadArgumentsErrorMessage
        = "Bad arguments provided. Controller: {0}, Action: {1}";

    public const string RemoteValidationErrorMessage
        = "Remote validation exception occurred. Controller: {0}, Action: {1}.";

    public const string RoleSeedingError 
        = "The role seeding process failed. Please view the result of the operation. RoleName: {0}";

    public const string AdminSeedingError
        = "The admin seeding process failed. Please view the result of the operation. email: {0}";

    public const string AdminUsernameNotFoundMessage = "Admin username not found.";
    public const string AdminEmailNotFoundMessage = "Admin email not found.";
    public const string AdminPasswordNotFoundMessage = "Admin password not found.";
}