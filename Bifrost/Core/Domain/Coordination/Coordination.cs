namespace Bifrost.Core.Domain.Coordination;

public class Coordination
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal? AvgScore { get; set; }
    public required Guid AssessmentSeasonId { get; set; }
    public AssessmentSeason.AssessmentSeasonMinimal assessmentSeason { get; set; } = null!;
    public required List<CoordinationMember.CoordinationMember> CoordinationMembers { get; set; }
}