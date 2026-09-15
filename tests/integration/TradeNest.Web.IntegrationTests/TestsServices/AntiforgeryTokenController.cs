using System.Net.Mime;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using TradeNest.Web.IntegrationTests.Models;

namespace TradeNest.Web.IntegrationTests.TestsServices;

/// <summary>
/// A class representing a controller for an HTTP GET resource that returns
/// valid CSRF tokens for use in integration tests.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class AntiforgeryTokenController : Controller
{
    /// <summary>
    /// The URL for the GET resource.
    /// </summary>
    private const string URL = "testing/xsrf-token";

    /// <summary>
    /// Gets the URI for the resource to get valid antiforgery tokens.
    /// </summary>
    public static Uri GetTokensUri => new Uri(URL, UriKind.Relative);

    /// <summary>
    /// Returns a JSON object containing valid antiforgery tokens.
    /// </summary>
    /// <param name="antiforgery">The <see cref="IAntiforgery"/> to use.</param>
    /// <param name="options">The <see cref="AntiforgeryOptions"/> to use.</param>
    /// <returns>
    /// An <see cref="AntiforgeryTokens"/> containing valid tokens for antiforgery.
    /// </returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json, Type = typeof(AntiforgeryTokens))]
    [Route(URL, Name = "AntiforgeryTokens")]
    public IActionResult GetAntiforgeryTokens(
        [FromServices] IAntiforgery antiforgery,
        [FromServices] IOptions<AntiforgeryOptions> options)
    {
        ArgumentNullException.ThrowIfNull(antiforgery);
        ArgumentNullException.ThrowIfNull(options);

        AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(this.HttpContext);

        AntiforgeryTokens model = new AntiforgeryTokens()
        {
            CookieName = options.Value!.Cookie!.Name!,
            CookieValue = tokens.CookieToken!,
            FormFieldName = options.Value.FormFieldName,
            HeaderName = tokens.HeaderName!,
            RequestToken = tokens.RequestToken!,
        };

        return Json(model);
    }
}
