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
        = "There was an issue while processing one or more of the products in your cart. Please review the details and take necessary action to proceed with your order.";

    internal const string OrderSubmittionSuccessMessage
        = "Your order was successfully submitted.";

    internal const string SuccessfullyRemovedUserMessage
        = "You have successfully removed the user.";
    
    internal const string SuccessfullyRemovedRoleMessage
        = "You have successfully removed the role.";

    internal const string CategoryDeletionSuccessFullMessage
        = "You have successfully removed the category. If there were any products that had the removed category as their category, they are now moved to the category with name \"{0}\"";

    internal const string CategoryDeletionSuccessMessage
        = "You have successfully removed the category.";

    internal const string RemovingDefaultCategoryMessage
        = "The \"{0}\" category is the default category. You can not remove the default category.";

    internal const string NoDefaultCategoryMessage
        = "There are products in this category. To be able to remove the category you need to create a new category called \"{0}\". All products from the removed category will be moved there.";

    internal const string SuccessfullyCreatedCategoryMessage
        = "You have successfully created the category with name \"{0}\".";

    internal const string UnexpectedErrorMessage
        = "Oops. Something went wrong. Please try again in a moment.";
}