namespace Bifrost.Core.Domain.AssessmentSeason;

public record AssessmentSeasonUpdateDto(string Period, DateTime? StartDateTime, DateTime? EndDateTime);