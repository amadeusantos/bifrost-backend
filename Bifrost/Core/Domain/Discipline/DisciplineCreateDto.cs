namespace Bifrost.Core.Domain.Discipline;

public record DisciplineCreateDto(
    Guid Id,
    string Name,
    string Code,
    Guid AssessmentSeasonId,
    Guid ProfessorId,
    List<Guid> Students);
