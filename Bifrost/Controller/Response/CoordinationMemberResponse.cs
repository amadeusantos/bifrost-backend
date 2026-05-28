using Bifrost.Controller.Response;
using Bifrost.Core.Domain.CoordinationMember;

namespace Bifrost.Response;

public class CoordinationMemberResponse(CoordinationMember coordinationMember)
{
    public Guid Id { get; set; } = coordinationMember.Id;
    public string Role { get; set; } = coordinationMember.Role;
    public Guid UserId { get; set; } = coordinationMember.UserId;
    public Guid CoordinationId { get; set; }  = coordinationMember.CoordinationId;
    public UserMinimalResponse User { get; set; } = new (coordinationMember.User);
}