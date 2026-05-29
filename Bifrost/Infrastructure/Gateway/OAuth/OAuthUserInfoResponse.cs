using System.Text.Json.Serialization;

namespace Bifrost.Infrastructure.Gateway.OAuth;

public class OAuthUserInfoResponse
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }
    
}