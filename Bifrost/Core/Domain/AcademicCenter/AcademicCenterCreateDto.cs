namespace Bifrost.Core.Domain.AcademicCenter;

public record AcademicCenterCreateDto(
    string Name,
    Guid AssessmentSeasonId,
    IEnumerable<AcademicCenterMemberDto> AcademicCenterMembers);
