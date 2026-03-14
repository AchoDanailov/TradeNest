namespace TradeNest.Services.Core.Utilities;

/// <summary>
/// This class provides with exception messages used in the services.
/// </summary>
internal static class ExceptionMessages
{
    internal const string UnhandledExceptionMessage = "An unhandled {0} occurred.";
    
    internal const string IdCantBeEmptyMessage = "Id can not be empty. {0}";

    internal const string NotFoundMessage = "{0} with id: {1} can not be found.";

    internal const string EmptyCartMessage
        = "User with id: {0} has no products added to his cart.";

    internal const string ProductNotFoundInCartMessage
        = "Product with id: {0} was not found in the user's cart.";

    internal const string CantBeZeroOrNegativeNumberMessage 
        = "{0} can not be zero or a negative number.";

    internal const string OwnerCantAddToCartProductHeOwnsMessage
        = "The owner of the product can not add the product to his order. userId: {0}, productId: {1}";

    internal const string OwnerCantRemoveProductHeOwnsFromCartMessage
        = "Owner of the product can not remove it from his cart. userId: {0}, productId: {1}";

    internal const string OrderAlreadySubmittedMessage 
        = "Order with id: {0} already is submitted.";

    internal const string CantDeleteAlreadyDeletedProduct
        = "Can not delete an already deleted product. userId: {0}, productId: {1}";
}