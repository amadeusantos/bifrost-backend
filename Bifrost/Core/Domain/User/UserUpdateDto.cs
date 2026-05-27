using Bifrost.Core.Domain.Enum;

namespace Bifrost.Core.Domain.User;

public record UserUpdateDto(string? FullName, UserProfileEnum Profile, bool? IsAdmin, Guid? CourseId);
