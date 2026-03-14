namespace TradeNest.Web.Utilities.Messages;

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

    internal const string CartModificationErrorMessage
        = "Oops. Something went wrong with the last attempt to modify your orders. Please try again in a moment.";

    internal const string ProblemWithCartProductMessage
        = "There was an issue while processing one or more of the products in your cart. Please review the product/s details and take necessary action to proceed with your order.";

    internal const string OrderSubmittionSuccessMessage
        = "Your order was successfully submitted.";

    internal const string UnexpectedErrorMessage
        = "Oops. Something went wrong. Please try again in a moment.";
}