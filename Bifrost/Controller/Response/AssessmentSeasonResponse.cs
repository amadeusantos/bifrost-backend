using Bifrost.Core.Domain.AssessmentSeason;

namespace Bifrost.Response;

public class AssessmentSeasonResponse(AssessmentSeason assessmentSeason)
{
    public Guid Id { get; set; } = assessmentSeason.Id;
    public string Period { get; set; } = assessmentSeason.Period;
    public DateTime? StartDateTime { get; set; } = assessmentSeason.StartDateTime;
    public DateTime? EndDateTime { get; set; } = assessmentSeason.EndDateTime;
    public CourseResponse Course { get; set; } = new (
        Id: assessmentSeason.Course.Id,
        Name: assessmentSeason.Course.Name,
        Code: assessmentSeason.Course.Code);
}
