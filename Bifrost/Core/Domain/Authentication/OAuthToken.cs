namespace Bifrost.Core.Domain.Authentication;

public class OAuthToken
{
    public required string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public required int ExpiresIn { get; set; }
    public int? RefreshTokenExpiresIn { get; set; }
    public required string TokenType { get; set; }
}