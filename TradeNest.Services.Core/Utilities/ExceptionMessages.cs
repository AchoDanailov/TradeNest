namespace TradeNest.Services.Core.Utilities;

internal static class ExceptionMessages
{
    internal const string IdCantBeEmptyMessage = "Id can not be empty. {0}";

    internal const string NotFoundMessage = "{0} with id: {1} can not be found.";

    internal const string CantBeZeroOrNegativeNumberMessage 
        = "{0} can not be zero or a negative number.";

    internal const string OwnerCantOrderProductHeOwnsMessage
        = "The owner of the product can not add the product to his order. userId: {0}, productId: {1}";

    internal const string OwnerCantRemoveProductHeOwnsFromOrderMessage
        = "Owner of the product can not remove it from his order's list. userId: {0}, productId: {1}";

    internal const string OrderAlreadySubmittedMessage 
        = "Order with id: {0} already is submitted.";

    internal const string CantDeleteAlreadyDeletedProduct
        = "Can not delete an already deleted product. userId: {0}, productId: {1}";
}