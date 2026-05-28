namespace Bifrost.Core.Domain.AcademicCenter;

public class AcademicCenterMember
{
    public Guid Id { get; set; }
    public required string Role { get; set; }
    public required Guid UserId { get; set; }
    public Guid AcademicCenterId { get; set; }
    public User.UserMinimal User { get; set; } = null!;
}