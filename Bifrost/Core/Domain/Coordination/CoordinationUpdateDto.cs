namespace Bifrost.Core.Domain.Coordination;

public record CoordinationUpdateDto(
    string Name, 
    List<CoordinationMemberDto> CoordinationMembers);