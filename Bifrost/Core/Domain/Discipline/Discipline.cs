using Bifrost.Core.Domain.User;

namespace Bifrost.Core.Domain.Discipline;

public class Discipline
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required Guid AssessmentSeasonId { get; set; }
    public required Guid ProfessorId { get; set; }
    public decimal? AvgScore { get; set; }
    public AssessmentSeason.AssessmentSeasonMinimal assessmentSeason { get; set; } = null!;
    public UserMinimal Professor { get; set; } = null!;
    public List<UserMinimal> Students { get; set; } = [];
}
