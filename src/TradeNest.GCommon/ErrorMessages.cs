namespace TradeNest.GCommon;

/// <summary>
/// Provides with error messages used for application errors.
/// </summary>
public static class ErrorMessages
{
    public const string IdCantBeEmptyMessage = "Id can not be empty. {0}";

    public const string NotFoundMessage = "{0} with id: {1} can not be found.";
    
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

    public const string FileNotFound = "File with path {0} was not found.";

    public const string ProductCreatedOnAfterApprovalTimeOfDecision
        = "Created on can not be after time of decision. productDtoId: {0}";
    
    public const string SeedingError
        = "The {0} seeding process failed. Please view the result of the operation or the inner exception.";

    public const string UserIsAlreadyAnAdminMessage = "A user can only be one admin.";
}