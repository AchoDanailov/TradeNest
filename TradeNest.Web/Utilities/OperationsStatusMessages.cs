namespace TradeNest.Web.Utilities;

/// <summary>
/// This class holds status messages used to notify the user about the status of attempted operations
/// available in the application.
/// </summary>
public static class OperationsStatusMessages
{
    public const string ProductCreationUnexpectedErrorMessage 
        = "Oops. Something went wrong while trying to add your new product. Please try again in a moment.";
    
    public const string ProductModificationUnexpectedErrorMessage 
        = "Oops. Something went wrong while trying save your changes. Please try again in a moment.";

    public const string ProductDeletionSuccessMessage = "The product was successfully deleted.";
}