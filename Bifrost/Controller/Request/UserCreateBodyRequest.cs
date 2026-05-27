using System.ComponentModel.DataAnnotations;
using Bifrost.Core.Domain.Enum;

namespace Bifrost.Request;

public record UserCreateBodyRequest(
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    Guid CourseId,
    UserProfileEnum Profile = UserProfileEnum.Student,
    bool IsAdmin = false,
    string? FullName = null);