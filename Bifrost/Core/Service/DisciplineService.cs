using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Discipline;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Exception.Discipline;
using Bifrost.Core.Port.Repository;

namespace Bifrost.Core.Service;

public class DisciplineService(
    IDisciplineRepository disciplineRepository,
    IAssessmentSeasonRepository assessmentSeasonRepository,
    IUserRepository userRepository) : IDisciplineService
{
    public async Task<Discipline> CreateDiscipline(DisciplineCreateDto dto)
    {
        var season = await assessmentSeasonRepository.FindById(dto.AssessmentSeasonId);
        if (season is null) throw new AssessmentSeasonNotFoundException();

        var professor = await userRepository.FindById(dto.ProfessorId);
        if (professor is not { Profile: UserProfileEnum.Professor }) throw new ProfessorProfileRequiredException();

        if (HasDuplicateStudents(dto.Students))
            throw new DisciplineStudentDuplicatedException();

        var allowedStudents = await userRepository.FindManyUsers(dto.Students, UserProfileEnum.Student, season.CourseId);

        var discipline = new Discipline
        {
            Name = dto.Name,
            Code = dto.Code,
            AssessmentSeasonId = dto.AssessmentSeasonId,
            ProfessorId = dto.ProfessorId,
            Students = allowedStudents.Cast<UserMinimal>().ToList()
        };
        return await disciplineRepository.Add(discipline);
    }

    public async Task<Discipline> GetDiscipline(Guid id)
    {
        Discipline? discipline = await disciplineRepository.FindById(id);
        return discipline ?? throw new DisciplineNotFoundException();
    }

    public async Task<Pagination<Discipline>> GetDisciplines(int page, int size)
    {
        return await disciplineRepository.GetDisciplines(page, size, null);
    }

    public async Task<Discipline> UpdateDiscipline(Guid id, DisciplineUpdateDto dto)
    {
        Discipline? discipline = await disciplineRepository.FindById(id);
        if (discipline is null) throw new DisciplineNotFoundException();

        var professor = await userRepository.FindById(dto.ProfessorId);
        if (professor is not { Profile: UserProfileEnum.Professor }) throw new ProfessorProfileRequiredException();

        if (HasDuplicateStudents(dto.Students))
            throw new DisciplineStudentDuplicatedException();

        var allowedStudents = await userRepository.FindManyUsers(dto.Students, UserProfileEnum.Student, discipline.assessmentSeason.CourseId);

        await disciplineRepository.RemoveDisciplineStudents(id);

        discipline.Name = dto.Name;
        discipline.Code = dto.Code;
        discipline.ProfessorId = dto.ProfessorId;
        discipline.Students = allowedStudents.Cast<UserMinimal>().ToList();
        return await disciplineRepository.Update(discipline);
    }

    public async Task DeleteDiscipline(Guid id)
    {
        if (!await disciplineRepository.IdExists(id)) throw new DisciplineNotFoundException();
        await disciplineRepository.DeleteById(id);
    }

    private static bool HasDuplicateStudents(List<Guid> studentIds)
    {
        return studentIds.Count != studentIds.Distinct().Count();
    }
}
