using System.ComponentModel.DataAnnotations;

namespace Bifrost.Request;

public record DisciplineUpdateBodyRequest(
    [Required] [MaxLength(32)] string Name,
    [Required] [MaxLength(32)] string Code,
    [Required] Guid ProfessorId,
    List<Guid> Students);
