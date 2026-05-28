using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record AcademicCenterCreateBodyRequest(
    [Required] [MaxLength(32)] string Name,
    [Required] Guid AssessmentSeasonId,
    [MinLength(1)] IEnumerable<AcademicCenterMemberBodyRequest> AcademicCenterMembers);
