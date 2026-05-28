using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AcademicCenter;

namespace Bifrost.Core.Adapter;

public interface IAcademicCenterService
{
    public Task<AcademicCenter> CreateAcademicCenter(AcademicCenterCreateDto createDto);

    public Task<AcademicCenter> GetAcademicCenter(Guid id);

    public Task<Pagination<AcademicCenter>> GetAcademicCenters(int page, int size, Guid? assessmentSeasonId);

    public Task<AcademicCenter> UpdateAcademicCenter(Guid id, AcademicCenterUpdateDto updateDto);

    public Task DeleteAcademicCenter(Guid id);
}
