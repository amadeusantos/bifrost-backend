using Bifrost.Core.Adapter;
using Bifrost.Core.Domain.Authentication;
using Bifrost.Request;
using Bifrost.Response;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost;

[ApiController]
[Route("auth")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("token")]
    public async Task<ActionResult<OAuthTokenResponse>> Authenticate([FromBody] AuthenticateBodyRequest request)
    {
        OAuthToken token = await authenticationService.Authenticate(new OAuthCodeDto(request.Code));
        return Ok(new OAuthTokenResponse(token));
    }
}
