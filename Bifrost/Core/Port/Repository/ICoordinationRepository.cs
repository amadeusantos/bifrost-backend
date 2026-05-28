using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Coordination;
using Bifrost.Core.Domain.User;

namespace Bifrost.Core.Port.Repository;

public interface ICoordinationRepository: IRepository<Coordination>
{
    public Task<Pagination<Coordination>> GetCoordinations(int page, int size);
    public Task<Coordination?> GetCoordinationByAssessmentSeasonId(Guid assessmentSeasonId);
    public Task<bool> ExistsCoordinationByAssessmentSeasonId(Guid assessmentSeasonId);
    public Task RemoveCoordinationMembers(Guid coordinationId);
}