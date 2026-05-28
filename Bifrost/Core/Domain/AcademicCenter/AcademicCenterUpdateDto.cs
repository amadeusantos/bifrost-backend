namespace Bifrost.Core.Domain.AcademicCenter;

public record AcademicCenterUpdateDto(
    string Name,
    List<AcademicCenterMemberDto> AcademicCenterMembers);
