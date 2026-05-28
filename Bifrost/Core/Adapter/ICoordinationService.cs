using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Coordination;

namespace Bifrost.Core.Adapter;

public interface ICoordinationService
{
    public Task<Coordination> CreateCoordination(CoordinationCreateDto coordinationCreateDto);
    
    public Task<Coordination> GetCoordination(Guid id);
    
    public Task<Pagination<Coordination>> GetCoordinations(int page, int size, Guid? assessmentSeasonId);
    
    public Task<Coordination> UpdateCoordination(Guid id, CoordinationUpdateDto coordinationUpdateDto);
    
    public Task DeleteCoordination(Guid id);
}