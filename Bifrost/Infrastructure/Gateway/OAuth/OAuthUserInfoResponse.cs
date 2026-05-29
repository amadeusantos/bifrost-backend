using System.Text.Json.Serialization;

namespace Bifrost.Infrastructure.Gateway.OAuth;

public class OAuthUserInfoResponse
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }
    
}