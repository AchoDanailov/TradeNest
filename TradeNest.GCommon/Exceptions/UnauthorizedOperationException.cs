namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when a user attempts to modify a resource he is not the
/// owner of.
/// </summary>
public class UnauthorizedOperationException : InvalidOperationException
{
    private const string DefaultMessage
        = "Unauthorized operation attempt. userId: {0}, {1}: {2}.";
    
    public Guid UserId { get; }
    public string ResourceName { get; }
    public object ResourceId { get; }

    public UnauthorizedOperationException(Guid userId, string resourceName, object resourceId)
        : base(string.Format(DefaultMessage, userId, resourceName, resourceId))
    {
        this.UserId = userId;
        this.ResourceName = resourceName;
        this.ResourceId = resourceId;
    }
}
