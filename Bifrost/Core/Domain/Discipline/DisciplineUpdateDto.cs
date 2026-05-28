namespace Bifrost.Core.Domain.Discipline;

public record DisciplineUpdateDto(Guid Id, string Name, string Code, Guid ProfessorId, List<Guid> Students);