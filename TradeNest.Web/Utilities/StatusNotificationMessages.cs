namespace TradeNest.Web.Utilities;

/// <summary>
/// This class holds status messages used to notify the user about the status of attempted operations
/// available in the application.
/// </summary>
internal static class StatusNotificationMessages
{
    internal const string ProductCreationUnexpectedErrorMessage 
        = "Oops. Something went wrong while trying to add your new product. Please try again in a moment.";
    
    internal const string ProductModificationUnexpectedErrorMessage 
        = "Oops. Something went wrong while trying save your changes. Please try again in a moment.";

    internal const string ProductDeletionSuccessMessage 
        = "The product was successfully deleted.";

    internal const string OrderModificationUnexpectedErrorMessage
        = "Oops. Something went wrong with the last attempt to modify your order. Please try again in a moment.";

    internal const string OrderSubmittionSuccessMessage
        = "Your order was successfully submitted.";

    internal const string UnexpectedErrorMessage
        = "Oops. Something went wrong. Please try again in a moment.";
}