using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AcademicCenter;

namespace Bifrost.Core.Port.Repository;

public interface IAcademicCenterRepository : IRepository<AcademicCenter>
{
    public Task<Pagination<AcademicCenter>> GetAcademicCenters(int page, int size, Guid? assessmentSeasonId);
    public Task<bool> ExistsAcademicCenterByAssessmentSeasonId(Guid assessmentSeasonId);
    public Task RemoveAcademicCenterMembers(Guid academicCenterId);
}
