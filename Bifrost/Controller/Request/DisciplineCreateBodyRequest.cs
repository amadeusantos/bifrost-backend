using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record DisciplineCreateBodyRequest(
    [Required] [MaxLength(32)] string Name,
    [Required] [MaxLength(32)] string Code,
    [Required] Guid AssessmentSeasonId,
    [Required] Guid ProfessorId,
    List<Guid> Students);
