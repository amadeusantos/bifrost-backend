using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Discipline;

namespace Bifrost.Core.Port.Repository;

public interface IDisciplineRepository : IRepository<Discipline>
{
    Task<Pagination<Discipline>> GetDisciplines(int page, int size, Guid? assessmentSeasonId);
    Task RemoveDisciplineStudents(Guid disciplineId);
}
