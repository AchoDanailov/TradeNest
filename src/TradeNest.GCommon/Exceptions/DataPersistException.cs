namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when data is not persisted successfully.
/// </summary>
public class DataPersistException : Exception
{
    private const string DefaultMessage = "Data persist exception occured.";
    
    public DataPersistException(string? message = null, Exception? innerException = null, params string[] data) 
        : base(
            message: !string.IsNullOrWhiteSpace(message)
                ? data.Any(s => !string.IsNullOrWhiteSpace(s)) 
                    ? string.Join(" ", message, string.Join(", ", data)) 
                    : message
                : data.Any(s => !string.IsNullOrWhiteSpace(s)) 
                    ? string.Join(" ", DefaultMessage, string.Join(", ", data)) 
                    : DefaultMessage,
            innerException: innerException)
    {
    }
}