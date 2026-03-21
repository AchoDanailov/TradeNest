namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when data is not persisted successfully.
/// </summary>
public class DataPersistException : Exception
{
    private const string DefaultMessage = "Data persist exception occurred.";

    public DataPersistException(params string[] data)
        : this(innerException: null, data: data)
    {
    }
    
    public DataPersistException(Exception? innerException, params string[] data)
        : base(
            message: data.Any(s => !string.IsNullOrWhiteSpace(s)) 
                ? string.Join(" ", DefaultMessage, string.Join(", ", data)) 
                : DefaultMessage,
            innerException: innerException)
    {
    }
}