namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when a user tries to add to his cart a product or tries
/// to submit a order containing a product that current status is NOT enabled.
/// </summary>
public class ProductDisabledException : InvalidOperationException
{
    private const string DefaultMessage
        = "Can not order a product that is not enabled. productId: {0}, userId: {1}";
    
    public Guid ProductId { get; }
    public Guid UserId { get; }

    public ProductDisabledException(Guid productId, Guid userId, Exception? innerException = null)
        : base(string.Format(DefaultMessage, productId, userId), innerException)
    {
        this.ProductId = productId;
        this.UserId = userId;
    }
}