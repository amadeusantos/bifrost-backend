using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record AssessmentSeasonCreateBodyRequest(
    [Required]
    [RegularExpression(@"^\d{4}\.\d$", ErrorMessage = "period must follow the format YYYY.N (e.g. 2026.1)")]
    string Period,
    [Required]
    Guid CourseId,
    DateTime? StartDateTime,
    DateTime? EndDateTime);
