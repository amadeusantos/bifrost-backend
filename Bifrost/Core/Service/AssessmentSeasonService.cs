using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;
using Bifrost.Core.Exception.AssessmentSeason;
using Bifrost.Core.Exception.Course;
using Bifrost.Core.Port.Repository;

namespace Bifrost.Core.Service;

public class AssessmentSeasonService(IAssessmentSeasonRepository repository, ICourseRepository courseRepository) : IAssessmentSeasonService
{
    public async Task<AssessmentSeason> CreateAssessmentSeason(AssessmentSeasonCreateDto assessmentSeasonCreateDto)
    {
        if (!await courseRepository.IdExists(assessmentSeasonCreateDto.CourseId))
        {
            throw new CourseNotFoundException();
        }
        if (await repository.ExistsAssessmentSeasonByPeriod(assessmentSeasonCreateDto.Period))
            throw new AssessmentSeasonPeriodAlreadyExistsException();

        AssessmentSeason assessmentSeason = new AssessmentSeason
        {
            Period =  assessmentSeasonCreateDto.Period,
            EndDateTime = assessmentSeasonCreateDto.EndDateTime,
            StartDateTime = assessmentSeasonCreateDto.StartDateTime,
            CourseId = assessmentSeasonCreateDto.CourseId
        };
        return await repository.Add(assessmentSeason);
    }

    public async Task<AssessmentSeason> GetAssessmentSeason(Guid id)
    {
        AssessmentSeason? assessmentSeason = await repository.FindById(id);
        return assessmentSeason ?? throw new AssessmentSeasonNotFoundException();
    }

    public async Task<List<AssessmentSeason>> GetAllAssessmentSeasons()
    {
        return await repository.GetAll();
    }

    public async Task<Pagination<AssessmentSeason>> GetAssessmentSeasons(int page, int size, Guid? courseId)
    {
        return await repository.GetAssessmentSeasons(page, size, courseId);
    }

    public async Task<AssessmentSeason> UpdateAssessmentSeason(Guid id, AssessmentSeasonUpdateDto assessmentSeasonUpdateDto)
    {
        AssessmentSeason? assessmentSeason = await repository.FindById(id);

        if (assessmentSeason is null) throw new AssessmentSeasonNotFoundException();

        if (assessmentSeason.Period != assessmentSeasonUpdateDto.Period && await repository.ExistsAssessmentSeasonByPeriod(assessmentSeasonUpdateDto.Period))
            throw new AssessmentSeasonPeriodAlreadyExistsException();

        assessmentSeason.Period = assessmentSeasonUpdateDto.Period;
        assessmentSeason.StartDateTime = assessmentSeasonUpdateDto.StartDateTime;
        assessmentSeason.EndDateTime = assessmentSeasonUpdateDto.EndDateTime;
        return await repository.Update(assessmentSeason);
    }

    public async Task DeleteAssessmentSeason(Guid id)
    {
        if (!await repository.IdExists(id)) throw new AssessmentSeasonNotFoundException();
        await repository.DeleteById(id);
    }
}
