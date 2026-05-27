using Bifrost.Core.Domain.Enum;

namespace Bifrost.Core.Domain.User;

public record UserCreateDto(string Email, Guid CourseId, UserProfileEnum Profile = UserProfileEnum.Student, bool IsAdmin = false, string? FullName = null);