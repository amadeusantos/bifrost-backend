namespace Bifrost.Core.Domain.Coordination;

public record CoordinationCreateDto(
    string Name, 
    Guid AssessmentSeasonId,
    IEnumerable<CoordinationMemberDto> CoordinationMembers
    );