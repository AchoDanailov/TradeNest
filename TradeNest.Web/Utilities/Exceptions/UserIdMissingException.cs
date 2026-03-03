namespace TradeNest.Web.Utilities.Exceptions;

internal class UserIdMissingException : InvalidOperationException
{
    private const string DefaultMessage
        = "UserId can not be null or empty. Controller: {0}, Action: {1}";

    internal string ControllerName { get; } = null!;
    internal string ActionName { get; } = null!;
    
    internal UserIdMissingException(string controllerName, string actionName)
        : base(string.Format(DefaultMessage, controllerName, actionName))
    {
    }
}