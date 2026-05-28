using Bifrost.Core.Domain.Enum;

namespace Bifrost.Core.Domain.User;

public class UserMinimal
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public required string Email { get; set; }
    public string? GoogleOpenid { get; set; }
    public UserProfileEnum Profile { get; set; }
    public bool IsAdmin { get; set; }
    public Guid? CourseId { get; set; }
}