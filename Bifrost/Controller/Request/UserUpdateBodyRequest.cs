using Bifrost.Core.Domain.Enum;

namespace Bifrost.Request;

public record UserUpdateBodyRequest(
    UserProfileEnum Profile,
    string? FullName = null,
    bool? IsAdmin = null,
    Guid? CourseId = null);
