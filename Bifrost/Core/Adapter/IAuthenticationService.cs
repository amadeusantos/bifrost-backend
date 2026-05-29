using Bifrost.Core.Domain.Authentication;

namespace Bifrost.Core.Adapter;

public interface IAuthenticationService
{
    Task<OAuthToken>  Authenticate(OAuthCodeDto codeDto);
}