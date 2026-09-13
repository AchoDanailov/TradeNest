using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using TradeNest.Web.IntegrationTests.Common;

namespace TradeNest.Web.IntegrationTests.TestsServices;

/// <summary>
/// Authentication handler that can be registered with a given authentication scheme, and used
/// in an authentication service.
/// </summary>
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <summary>
    /// Method that creates claims, claims principal and ticket for that principal,
    /// then proceeds to authenticate the ticket.
    /// </summary>
    /// <returns>Task holding an <see cref="AuthenticateResult.Success"/> object.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        IEnumerable<Claim> claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, TestsConstants.Auth.UserIdGuid)
        };
        ClaimsIdentity identity = new ClaimsIdentity(claims, TestsConstants.Auth.Username);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, TestsConstants.Auth.Scheme);

        AuthenticateResult result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}