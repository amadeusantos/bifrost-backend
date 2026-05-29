namespace Bifrost.Core.Domain.Authentication;

public class OAuthUserInfo
{
    public string? Name { get; set; }
    public required string Email { get; set; }
    public required string Id { get; set; }
}