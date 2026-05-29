using Bifrost.Core.Domain.Authentication;

namespace Bifrost.Response;

public class OAuthTokenResponse(OAuthToken token)
{
    public string AccessToken { get; init; } = token.AccessToken;
    public string? RefreshToken { get; init; } = token.RefreshToken;
    public int ExpiresIn { get; init; } = token.ExpiresIn;
    public int? RefreshTokenExpiresIn { get; init; } = token.RefreshTokenExpiresIn;
    public string TokenType { get; init; } = token.TokenType;
}
