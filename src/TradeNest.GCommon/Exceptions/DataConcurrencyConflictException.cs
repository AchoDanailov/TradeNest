namespace TradeNest.GCommon.Exceptions;

/// <summary>
/// The exception that is thrown when there is a conflict between the state of the data in
/// the data source and the modified data that is being requested to be persisted.
/// </summary>
public class DataConcurrencyConflictException : InvalidOperationException
{
    private const string DefaultMessage
        = "An concurrency exception occured while trying to persist the data.";

    public DataConcurrencyConflictException(string? message = null, Exception? innerException = null, params string[] data)
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

