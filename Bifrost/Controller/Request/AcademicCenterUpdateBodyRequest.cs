using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record AcademicCenterUpdateBodyRequest(
    [Required] [MaxLength(32)] string Name,
    [MinLength(1)] IEnumerable<AcademicCenterMemberBodyRequest> AcademicCenterMembers);
