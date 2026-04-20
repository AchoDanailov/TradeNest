namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when a user tries to add to his cart a product or tries
/// to submit a order containing a product that is currently not approved.
/// </summary>
public class ProductNotApprovedException : InvalidOperationException
{
    private const string DefaultMessage
        = "Can not add to cart a product that is not approved. userId: {0}, productId: {1}";
    
    public Guid ProductId { get; }
    public Guid UserId { get; }

    public ProductNotApprovedException(Guid productId, Guid userId, Exception? innerException = null)
        : base(string.Format(DefaultMessage, productId, userId), innerException)
    {
        this.ProductId = productId;
        this.UserId = userId;
    }
}