using System.Security.Claims;
using System.Text.Encodings.Web;
using Bifrost.Core.Port.Gateway;
using Bifrost.Core.Port.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Bifrost.Infrastructure.Auth;

public class GoogleOAuthAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOAuthGateway oAuthGateway,
    IUserRepository userRepository)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return AuthenticateResult.NoResult();

        var accessToken = authHeader["Bearer ".Length..].Trim();

        if (string.IsNullOrEmpty(accessToken))
            return AuthenticateResult.NoResult();

        try
        {
            var userInfo = await oAuthGateway.GetUserInfo(accessToken);

            var user = await userRepository.FindByGoogleId(userInfo.Id);
            if (user is null)
                return AuthenticateResult.Fail("User not registered.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Profile.ToString()),
                new Claim("CourseId", user.CourseId.ToString() ?? ""),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex.Message);
        }
    }
}
