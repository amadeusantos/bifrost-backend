using Bifrost.Core.Domain.Authentication;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Exception.Authentication;
using Bifrost.Core.Port.Gateway;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using FluentAssertions;
using NSubstitute;

namespace Bifrost.Test.Core;

public class AuthenticationServiceTests
{
    private readonly IOAuthGateway _oAuthGateway = Substitute.For<IOAuthGateway>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly AuthenticationService _sut;

    private static readonly OAuthToken ValidToken = new()
    {
        AccessToken = "access-token",
        ExpiresIn = 3600,
        TokenType = "Bearer",
        IdToken = "id-token"
    };

    private static readonly OAuthUserInfo ValidUserInfo = new()
    {
        Id = "google-id-123",
        Email = "user@test.com",
        Name = "Test User"
    };

    public AuthenticationServiceTests()
    {
        _sut = new AuthenticationService(_userRepository, _oAuthGateway);
    }

    // --- InvalidOAuthCodeException ---

    [Fact]
    public async Task Authenticate_WhenGatewayThrowsInvalidCode_PropagatesException()
    {
        var dto = new OAuthCodeDto("invalid-code");
        _oAuthGateway.RequestToken(dto.Code).Returns(Task.FromException<OAuthToken>(new InvalidOAuthCodeException()));

        var act = () => _sut.Authenticate(dto);

        await act.Should().ThrowAsync<InvalidOAuthCodeException>();
    }

    // --- InsufficientOAuthScopeException ---

    [Fact]
    public async Task Authenticate_WhenGatewayThrowsInsufficientScope_PropagatesException()
    {
        var dto = new OAuthCodeDto("valid-code");
        _oAuthGateway.RequestToken(dto.Code).Returns(Task.FromException<OAuthToken>(new InsufficientOAuthScopeException()));

        var act = () => _sut.Authenticate(dto);

        await act.Should().ThrowAsync<InsufficientOAuthScopeException>();
    }

    // --- User not found → create ---

    [Fact]
    public async Task Authenticate_WhenUserNotFound_CreatesNewUser()
    {
        var dto = new OAuthCodeDto("valid-code");
        _oAuthGateway.RequestToken(dto.Code).Returns(ValidToken);
        _oAuthGateway.GetUserInfo(ValidToken.AccessToken).Returns(ValidUserInfo);
        _userRepository.FindByEmail(ValidUserInfo.Email).Returns((User?)null);
        _userRepository.Add(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());

        await _sut.Authenticate(dto);

        await _userRepository.Received(1).Add(Arg.Is<User>(u =>
            u.Email == ValidUserInfo.Email &&
            u.FullName == ValidUserInfo.Name &&
            u.GoogleOpenid == ValidUserInfo.Id &&
            u.Profile == UserProfileEnum.Student));
    }

    // --- User exists, GoogleOpenid null → update ---

    [Fact]
    public async Task Authenticate_WhenUserExistsWithoutGoogleOpenid_UpdatesUser()
    {
        var dto = new OAuthCodeDto("valid-code");
        var existingUser = new User { Email = ValidUserInfo.Email, GoogleOpenid = null };

        _oAuthGateway.RequestToken(dto.Code).Returns(ValidToken);
        _oAuthGateway.GetUserInfo(ValidToken.AccessToken).Returns(ValidUserInfo);
        _userRepository.FindByEmail(ValidUserInfo.Email).Returns(existingUser);

        await _sut.Authenticate(dto);

        await _userRepository.Received(1).Update(Arg.Is<User>(u =>
            u.GoogleOpenid == ValidUserInfo.Id &&
            u.FullName == ValidUserInfo.Name));
    }

    // --- User exists, GoogleOpenid set → no update ---

    [Fact]
    public async Task Authenticate_WhenUserExistsWithGoogleOpenid_DoesNotUpdateUser()
    {
        var dto = new OAuthCodeDto("valid-code");
        var existingUser = new User { Email = ValidUserInfo.Email, GoogleOpenid = "already-set" };

        _oAuthGateway.RequestToken(dto.Code).Returns(ValidToken);
        _oAuthGateway.GetUserInfo(ValidToken.AccessToken).Returns(ValidUserInfo);
        _userRepository.FindByEmail(ValidUserInfo.Email).Returns(existingUser);

        await _sut.Authenticate(dto);

        await _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }

    // --- Returns token ---

    [Fact]
    public async Task Authenticate_WhenValid_ReturnsTokenFromGateway()
    {
        var dto = new OAuthCodeDto("valid-code");
        var existingUser = new User { Email = ValidUserInfo.Email, GoogleOpenid = "set" };

        _oAuthGateway.RequestToken(dto.Code).Returns(ValidToken);
        _oAuthGateway.GetUserInfo(ValidToken.AccessToken).Returns(ValidUserInfo);
        _userRepository.FindByEmail(ValidUserInfo.Email).Returns(existingUser);

        var result = await _sut.Authenticate(dto);

        result.Should().BeEquivalentTo(ValidToken);
    }
}
