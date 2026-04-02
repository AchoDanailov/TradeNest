namespace TradeNest.Web.Utilities.Exceptions;

/// <summary>
/// The exception that is thrown when the user's id is missing.
/// </summary>
internal class UserIdMissingException : InvalidOperationException
{
    private const string DefaultMessage
        = "UserId can not be null or empty. Area: {0}, Controller: {1}, Action: {2}";

    internal string? AreaName { get; }
    internal string ControllerName { get; }
    internal string ActionName { get; }
    
    internal UserIdMissingException(string controllerName, string actionName, string? areaName = null)
        : base(string.Format(DefaultMessage, areaName, controllerName, actionName))
    {
        this.AreaName = areaName;
        this.ControllerName = controllerName;
        this.ActionName = actionName;
    }
}