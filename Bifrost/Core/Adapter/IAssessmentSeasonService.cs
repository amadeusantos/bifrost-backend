using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;

namespace Bifrost.Core.Adapter;

public interface IAssessmentSeasonService
{
    public Task<AssessmentSeason> CreateAssessmentSeason(AssessmentSeasonCreateDto assessmentSeasonCreateDto);

    public Task<AssessmentSeason> GetAssessmentSeason(Guid id);

    public Task<List<AssessmentSeason>> GetAllAssessmentSeasons();

    public Task<Pagination<AssessmentSeason>> GetAssessmentSeasons(int page, int size, Guid? courseId);

    public Task<AssessmentSeason> UpdateAssessmentSeason(Guid id, AssessmentSeasonUpdateDto assessmentSeasonUpdateDto);

    public Task DeleteAssessmentSeason(Guid id);
}
