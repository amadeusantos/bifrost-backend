using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;

namespace Bifrost.Response;

public class UserResponse(User user)
{
    public Guid Id { get; init; } = user.Id;
    public string? FullName { get; init; } = user.FullName;
    public string Email { get; init; } = user.Email;
    public UserProfileEnum Profile { get; init; } =  user.Profile;
    public bool IsAdmin { get; init; } = user.IsAdmin;
    public CourseResponse? Course { get; set; } = user.Course != null
        ? new (Id: user.Course.Id, Name: user.Course.Name, Code: user.Course.Code)
        : null;
}