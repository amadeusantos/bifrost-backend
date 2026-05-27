using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;

namespace Bifrost.Core.Port.Repository;

public interface IAssessmentSeasonRepository : IRepository<AssessmentSeason>
{
    public Task<Pagination<AssessmentSeason>> GetAssessmentSeasons(int page, int size, Guid? courseId);
    public Task<bool> ExistsAssessmentSeasonByPeriod(string period);
}
