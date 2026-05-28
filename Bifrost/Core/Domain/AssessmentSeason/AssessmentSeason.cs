namespace Bifrost.Core.Domain.AssessmentSeason;

public class AssessmentSeason: AssessmentSeasonMinimal
{
    public Course.Course Course { get; set; } = null!;
}
