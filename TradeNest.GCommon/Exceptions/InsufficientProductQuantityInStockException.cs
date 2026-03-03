namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when a user attempt to either add product to his order or
/// submit an order with quantity more than the quantity in stock of the given product.
/// </summary>
public class InsufficientProductQuantityInStockException : InvalidOperationException
{
    private const string DefaultMessage 
        = "Not enough quantity in stock to add to the user order. userId: {0}, productId: {1}, quantity in stock: {2}, quantity attempted to add to order: {3}.";
    
    public Guid UserId { get; }
    public Guid ProductId { get; }
    public int ProductQtyInStock { get; }
    public int ProductQtyRequested { get; }

    public InsufficientProductQuantityInStockException(Guid userId, Guid productId,
        int productQtyInStock, int productQtyRequested)
        : base(string.Format(DefaultMessage, userId, productId, productQtyInStock, productQtyRequested))
    {
        this.UserId = userId;
        this.ProductId = productId;
        this.ProductQtyInStock = productQtyInStock;
        this.ProductQtyRequested = productQtyRequested;
    }
}