using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record AssessmentSeasonUpdateBodyRequest(
    [Required]
    [RegularExpression(@"^\d{4}\.\d$", ErrorMessage = "period must follow the format YYYY.N (e.g. 2026.1)")]
    string Period,
    DateTime? StartDateTime,
    DateTime? EndDateTime);