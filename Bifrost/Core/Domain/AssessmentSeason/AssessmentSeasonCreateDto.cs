namespace Bifrost.Core.Domain.AssessmentSeason;

public record AssessmentSeasonCreateDto(string Period, Guid CourseId, DateTime? StartDateTime, DateTime? EndDateTime);
