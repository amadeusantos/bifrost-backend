using Bifrost.Core.Adapter;
using Bifrost.Core.Domain.Authentication;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Port.Gateway;
using Bifrost.Core.Port.Repository;

namespace Bifrost.Core.Service;

public class AuthenticationService(IUserRepository userRepository, IOAuthGateway authGateway): IAuthenticationService
{
    public async Task<OAuthToken> Authenticate(OAuthCodeDto codeDto)
    {
        var authToken = await authGateway.RequestToken(codeDto.Code);

        var userInfo = await authGateway.GetUserInfo(authToken.AccessToken);

        var user = await userRepository.FindByEmail(userInfo.Email) ?? await userRepository.Add(new User
        {
            Email = userInfo.Email,
            Profile = UserProfileEnum.Student,
            FullName = userInfo.Name,
            GoogleOpenid = userInfo.Id
        });

        if (user.GoogleOpenid == null)
        {
            user.FullName = userInfo.Name ?? user.FullName;
            user.GoogleOpenid = userInfo.Id;
            await userRepository.Update(user);
        }
        
        return authToken;
    }
}