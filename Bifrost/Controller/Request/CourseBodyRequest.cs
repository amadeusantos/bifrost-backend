using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record CourseBodyRequest(
    [Required]
    [MaxLength(64)]
    string Name,
    [Required]
    [MaxLength(32)]
    string Code);