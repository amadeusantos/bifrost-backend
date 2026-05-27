using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Exception.Course;
using Bifrost.Core.Exception.User;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using FluentAssertions;
using NSubstitute;

namespace Bifrost.Test.Core;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ICourseRepository _courseRepository = Substitute.For<ICourseRepository>();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_userRepository, _courseRepository);
    }

    // --- Create ---

    [Fact]
    public async Task CreateUser_WhenCourseDoesNotExist_ThrowsCourseNotFoundException()
    {
        var dto = new UserCreateDto("user@test.com", Guid.NewGuid());
        _courseRepository.IdExists(dto.CourseId).Returns(false);

        var act = () => _sut.CreateUser(dto);

        await act.Should().ThrowAsync<CourseNotFoundException>();
    }

    [Fact]
    public async Task CreateUser_WhenEmailAlreadyExists_ThrowsUserEmailAlreadyExistsException()
    {
        var dto = new UserCreateDto("user@test.com", Guid.NewGuid());
        _courseRepository.IdExists(dto.CourseId).Returns(true);
        _userRepository.EmailExists(dto.Email).Returns(true);

        var act = () => _sut.CreateUser(dto);

        await act.Should().ThrowAsync<UserEmailAlreadyExistsException>();
    }

    [Fact]
    public async Task CreateUser_WhenValid_ReturnsSavedUser()
    {
        var courseId = Guid.NewGuid();
        var dto = new UserCreateDto("user@test.com", courseId, UserProfileEnum.Student, false, "Nome");
        var saved = new User { Id = Guid.NewGuid(), Email = dto.Email, CourseId = courseId };

        _courseRepository.IdExists(courseId).Returns(true);
        _userRepository.EmailExists(dto.Email).Returns(false);
        _userRepository.Add(Arg.Any<User>()).Returns(saved);

        var result = await _sut.CreateUser(dto);

        result.Should().BeEquivalentTo(saved);
    }

    // --- GetById ---

    [Fact]
    public async Task GetUser_WhenNotFound_ThrowsUserNotFoundException()
    {
        var id = Guid.NewGuid();
        _userRepository.FindById(id).Returns((User?)null);

        var act = () => _sut.GetUser(id);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task GetUser_WhenExists_ReturnsUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, Email = "user@test.com" };
        _userRepository.FindById(id).Returns(user);

        var result = await _sut.GetUser(id);

        result.Should().BeEquivalentTo(user);
    }

    // --- Paginate ---

    [Fact]
    public async Task GetUsers_WithoutFilters_ReturnsPaginatedResult()
    {
        var pagination = new Pagination<User>(1, 10, 0, []);
        _userRepository.GetUsers(1, 10, null, null).Returns(pagination);

        var result = await _sut.GetUsers(1, 10, null, null);

        result.Should().BeEquivalentTo(pagination);
    }

    [Fact]
    public async Task GetUsers_WithFilters_ForwardsFiltersToRepository()
    {
        var courseId = Guid.NewGuid();
        var pagination = new Pagination<User>(1, 10, 0, []);
        _userRepository.GetUsers(1, 10, UserProfileEnum.Student, courseId).Returns(pagination);

        var result = await _sut.GetUsers(1, 10, UserProfileEnum.Student, courseId);

        result.Should().BeEquivalentTo(pagination);
        await _userRepository.Received(1).GetUsers(1, 10, UserProfileEnum.Student, courseId);
    }

    // --- Update ---

    [Fact]
    public async Task UpdateUser_WhenNotFound_ThrowsUserNotFoundException()
    {
        var id = Guid.NewGuid();
        _userRepository.FindById(id).Returns((User?)null);

        var act = () => _sut.UpdateUser(id, new UserUpdateDto("Nome", UserProfileEnum.Student, null, null));

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task UpdateUser_WhenCourseIdProvidedAndCourseDoesNotExist_ThrowsCourseNotFoundException()
    {
        var id = Guid.NewGuid();
        var newCourseId = Guid.NewGuid();
        var user = new User { Id = id, Email = "user@test.com" };

        _userRepository.FindById(id).Returns(user);
        _courseRepository.IdExists(newCourseId).Returns(false);

        var act = () => _sut.UpdateUser(id, new UserUpdateDto(null, UserProfileEnum.Professor, null, newCourseId));

        await act.Should().ThrowAsync<CourseNotFoundException>();
    }

    [Fact]
    public async Task UpdateUser_WhenCourseIdIsNull_DoesNotValidateCourse()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, Email = "user@test.com", CourseId = Guid.NewGuid() };
        var dto = new UserUpdateDto("Novo Nome", UserProfileEnum.Professor, true, null);
        var updated = new User { Id = id, Email = user.Email, FullName = dto.FullName, Profile = dto.Profile, IsAdmin = true, CourseId = user.CourseId };

        _userRepository.FindById(id).Returns(user);
        _userRepository.Update(Arg.Any<User>()).Returns(updated);

        await _sut.UpdateUser(id, dto);

        await _courseRepository.DidNotReceive().IdExists(Arg.Any<Guid>());
    }

    [Fact]
    public async Task UpdateUser_WhenIsAdminIsNull_KeepsCurrentIsAdmin()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, Email = "user@test.com", IsAdmin = true };
        var dto = new UserUpdateDto(null, UserProfileEnum.Student, null, null);

        _userRepository.FindById(id).Returns(user);
        _userRepository.Update(Arg.Any<User>()).Returns(callInfo => (User?)callInfo.Arg<User>());

        var result = await _sut.UpdateUser(id, dto);

        result.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUser_WhenCourseIdIsNull_KeepsCurrentCourseId()
    {
        var id = Guid.NewGuid();
        var originalCourseId = Guid.NewGuid();
        var user = new User { Id = id, Email = "user@test.com", CourseId = originalCourseId };
        var dto = new UserUpdateDto(null, UserProfileEnum.Student, null, null);

        _userRepository.FindById(id).Returns(user);
        _userRepository.Update(Arg.Any<User>()).Returns(callInfo => (User?)callInfo.Arg<User>());

        var result = await _sut.UpdateUser(id, dto);

        result.CourseId.Should().Be(originalCourseId);
    }

    [Fact]
    public async Task UpdateUser_WhenValid_ReturnsUpdatedUser()
    {
        var id = Guid.NewGuid();
        var newCourseId = Guid.NewGuid();
        var user = new User { Id = id, Email = "user@test.com", IsAdmin = false };
        var dto = new UserUpdateDto("Novo Nome", UserProfileEnum.Professor, true, newCourseId);
        var updated = new User { Id = id, Email = user.Email, FullName = dto.FullName, Profile = dto.Profile, IsAdmin = true, CourseId = newCourseId };

        _userRepository.FindById(id).Returns(user);
        _courseRepository.IdExists(newCourseId).Returns(true);
        _userRepository.Update(Arg.Any<User>()).Returns(updated);

        var result = await _sut.UpdateUser(id, dto);

        result.FullName.Should().Be(dto.FullName);
        result.Profile.Should().Be(dto.Profile);
        result.IsAdmin.Should().BeTrue();
        result.CourseId.Should().Be(newCourseId);
    }

    // --- Delete ---

    [Fact]
    public async Task DeleteUser_WhenNotFound_ThrowsUserNotFoundException()
    {
        var id = Guid.NewGuid();
        _userRepository.IdExists(id).Returns(false);

        var act = () => _sut.DeleteUser(id);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task DeleteUser_WhenExists_CallsDeleteById()
    {
        var id = Guid.NewGuid();
        _userRepository.IdExists(id).Returns(true);

        await _sut.DeleteUser(id);

        await _userRepository.Received(1).DeleteById(id);
    }
}
