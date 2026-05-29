using Bifrost.Core.Domain.Authentication;

namespace Bifrost.Core.Port.Gateway;

public interface IOAuthGateway
{
    Task<OAuthToken> RequestToken(string code);
    Task<OAuthUserInfo> GetUserInfo(string accessToken);
}