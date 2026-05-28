using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Discipline;

namespace Bifrost.Core.Adapter;

public interface IDisciplineService
{
    public Task<Discipline> CreateDiscipline(DisciplineCreateDto disciplineCreateDto);

    public Task<Discipline> GetDiscipline(Guid id);

    public Task<Pagination<Discipline>> GetDisciplines(int page, int size);

    public Task<Discipline> UpdateDiscipline(Guid id, DisciplineUpdateDto disciplineUpdateDto);

    public Task DeleteDiscipline(Guid id);
}
