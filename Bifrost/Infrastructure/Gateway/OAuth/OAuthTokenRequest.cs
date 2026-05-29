using System.Text.Json.Serialization;

namespace Bifrost.Infrastructure.Gateway.OAuth;

public class OAuthTokenRequest
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; set; }
    [JsonPropertyName("redirect_uri")]
    public required string RedirectUri { get; set; }
    [JsonPropertyName("grant_type")]
    public required string GrantType { get; set; }
}