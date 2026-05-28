using Bifrost.Core.Domain.AssessmentSeason;

namespace Bifrost.Response;

public class AssessmentSeasonMinimalResponse(AssessmentSeasonMinimal assessmentSeason)
{
    public Guid Id { get; set; } = assessmentSeason.Id;
    public string Period { get; set; } = assessmentSeason.Period;
    public DateTime? StartDateTime { get; set; } = assessmentSeason.StartDateTime;
    public DateTime? EndDateTime { get; set; } = assessmentSeason.EndDateTime;
}