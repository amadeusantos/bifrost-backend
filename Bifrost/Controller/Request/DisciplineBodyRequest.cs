using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record DisciplineBodyRequest(
    [MaxLength(32)] string name,
    [MaxLength(32)] string code,
    [RegularExpression(@"^\d{4}\.\d$", ErrorMessage = "period must follow the format YYYY.N (e.g. 2026.1)")]
    string period,
    Guid professorId,
    decimal? score);
