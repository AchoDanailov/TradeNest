namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when a given resource is not found. Usually this resource
/// is the main resource being targeted in the given context.
/// </summary>
public class ResourceNotFoundException : InvalidOperationException
{
    private const string DefaultMessage = "{0} with id: {1} was not found.";
    
    public string ResourceName { get; }
    public object ResourceId { get; }

    public ResourceNotFoundException(string resourceName, object resourceId,
        Exception? innerException = null) 
        : base(string.Format(DefaultMessage, resourceName, resourceId), innerException)
    {
        this.ResourceName = resourceName;
        this.ResourceId = resourceId;
    }
}