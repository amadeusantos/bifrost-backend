namespace Bifrost.Core.Domain.AssessmentSeason;

public class AssessmentSeason
{
    public Guid Id { get; set; }
    public required string Period { get; set; }
    public required Guid CourseId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public Course.Course Course { get; set; } = null!;
}
