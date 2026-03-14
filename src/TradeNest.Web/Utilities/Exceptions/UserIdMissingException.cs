namespace TradeNest.Web.Utilities.Exceptions;

/// <summary>
/// The exception that is thrown when the user's id is missing.
/// </summary>
internal class UserIdMissingException : InvalidOperationException
{
    private const string DefaultMessage
        = "UserId can not be null or empty. Controller: {0}, Action: {1}";

    internal string ControllerName { get; }
    internal string ActionName { get; }
    
    internal UserIdMissingException(string controllerName, string actionName)
        : base(string.Format(DefaultMessage, controllerName, actionName))
    {
        this.ControllerName = controllerName;
        this.ActionName = actionName;
    }
}