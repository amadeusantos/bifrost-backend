using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record CoordinationCreateBodyRequest(
    [Required] [MaxLength(32)] string Name,
    [Required] Guid AssessmentSeasonId,
    [MinLength(1)]
    IEnumerable<CoordinationMemberBodyRequest> CoordinationMembers);