namespace Bifrost.Core.Domain.AcademicCenter;

public class AcademicCenter
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal? AvgScore { get; set; }
    public required Guid AssessmentSeasonId { get; set; }
    public AssessmentSeason.AssessmentSeasonMinimal assessmentSeason { get; set; } = null!;
    public required List<AcademicCenterMember> AcademicCenterMembers { get; set; }
}