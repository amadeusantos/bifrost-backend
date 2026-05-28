using Bifrost.Controller.Response;
using Bifrost.Core.Domain.AcademicCenter;

namespace Bifrost.Response;

public class AcademicCenterMemberResponse(AcademicCenterMember member)
{
    public Guid Id { get; set; } = member.Id;
    public string Role { get; set; } = member.Role;
    public Guid UserId { get; set; } = member.UserId;
    public Guid AcademicCenterId { get; set; } = member.AcademicCenterId;
    public UserMinimalResponse User { get; set; } = new(member.User);
}
