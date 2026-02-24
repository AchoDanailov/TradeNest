namespace TradeNest.GCommon.Exceptions;

public class ResourceNotFoundException : InvalidOperationException
{
    private const string DefaultMessage = "{0} with id: {1} was not found.";
    
    public string ResourceName { get; }
    public object ResourceId { get; }

    public ResourceNotFoundException(string resourceName, object resourceId)
        : base(string.Format(DefaultMessage, resourceName, resourceId))
    {
        this.ResourceName = resourceName;
        this.ResourceId = resourceId;
    }
}