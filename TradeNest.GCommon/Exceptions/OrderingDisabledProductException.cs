namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when a user tries to add to his order a product or tries
/// to submit a order containing a product that current status is NOT enabled.
/// </summary>
public class OrderingDisabledProductException : InvalidOperationException
{
    private const string DefaultMessage
        = "Can not order a product that is not enabled. productId: {0}, userId: {1}, orderId: {2}";
    
    public Guid ProductId { get; }
    public Guid UserId { get; }
    public Guid OrderId { get; }

    public OrderingDisabledProductException(Guid productId, Guid userId, Guid orderId)
        : base(string.Format(DefaultMessage, productId, orderId, userId))
    {
        this.ProductId = productId;
        this.OrderId = orderId;
        this.UserId = userId;
    }
}