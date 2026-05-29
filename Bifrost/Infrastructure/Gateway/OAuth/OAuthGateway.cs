using System.Net.Http.Headers;
using System.Text.Json;
using Bifrost.Core.Domain.Authentication;
using Bifrost.Core.Port.Gateway;

namespace Bifrost.Infrastructure.Gateway.OAuth;

public class OAuthGateway(HttpClient httpClient, IConfiguration configuration) : IOAuthGateway
{
    private const string Oauth2GoogleUrl = "https://oauth2.googleapis.com";
    private const string GoogleApi = "https://www.googleapis.com/oauth2/v2";

    private readonly string _clientId = configuration["Google:OAuth:ClientId"]
                                        ?? throw new InvalidOperationException("Missing configuration: Google:OAuth:ClientId");
    private readonly string _clientSecret = configuration["Google:OAuth:ClientSecret"]
                                            ?? throw new InvalidOperationException("Missing configuration: Google:OAuth:ClientSecret");
    private readonly string _redirectUri = configuration["Google:OAuth:RedirectUri"]
                                           ?? throw new InvalidOperationException("Missing configuration: Google:OAuth:RedirectUri");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<OAuthToken> RequestToken(string code)
    {
        var body = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret),
            new KeyValuePair<string, string>("redirect_uri", _redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        ]);

        var response = await httpClient.PostAsync($"{Oauth2GoogleUrl}/token", body);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await JsonSerializer.DeserializeAsync<OAuthTokenResponse>(
            await response.Content.ReadAsStreamAsync(), JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize OAuth token response");

        return new OAuthToken
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
            RefreshTokenExpiresIn = tokenResponse.RefreshTokenExpiresIn,
            TokenType = tokenResponse.TokenType
        };
    }

    public async Task<OAuthUserInfo> GetUserInfo(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{GoogleApi}/userinfo?alt=json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var userInfoResponse = await JsonSerializer.DeserializeAsync<OAuthUserInfoResponse>(
            await response.Content.ReadAsStreamAsync(), JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize OAuth user info response");

        return new OAuthUserInfo
        {
            Id = userInfoResponse.Id,
            Email = userInfoResponse.Email,
            Name = userInfoResponse.Name
        };
    }
}
