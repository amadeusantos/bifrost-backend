namespace Bifrost.Core.Domain.Coordination;

public class CoordinationMember
{
    public Guid Id { get; set; }
    public required string Role { get; set; }
    public required Guid UserId { get; set; }
    public Guid CoordinationId { get; set; }
    public User.UserMinimal User { get; set; } = null!;
}