namespace Bifrost.Core.Domain.AssessmentSeason;

public class AssessmentSeasonMinimal
{
    public Guid Id { get; set; }
    public required string Period { get; set; }
    public required Guid CourseId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}