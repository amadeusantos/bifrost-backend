using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AcademicCenter;
using Bifrost.Request;
using Bifrost.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost;

[Authorize]
[ApiController]
[Route("academic-centers")]
public class AcademicCenterController(IAcademicCenterService academicCenterService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<AcademicCenter, AcademicCenterResponse>>> Paginate(
        [FromQuery] PaginationQueryRequest paginationQueryRequest,
        [FromQuery] Guid? assessmentSeasonId)
    {
        Pagination<AcademicCenter> pagination =
            await academicCenterService.GetAcademicCenters(paginationQueryRequest.Page, paginationQueryRequest.Size, assessmentSeasonId);

        return new PaginationResponse<AcademicCenter, AcademicCenterResponse>(
            pagination, ac => new AcademicCenterResponse(ac));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AcademicCenterResponse>> GetById(Guid id)
    {
        AcademicCenter academicCenter = await academicCenterService.GetAcademicCenter(id);
        return new AcademicCenterResponse(academicCenter);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<AcademicCenterResponse>> Create(
        [FromBody] AcademicCenterCreateBodyRequest request)
    {
        AcademicCenter academicCenter = await academicCenterService.CreateAcademicCenter(
            new AcademicCenterCreateDto(
                request.Name,
                request.AssessmentSeasonId,
                request.AcademicCenterMembers.Select(m => new AcademicCenterMemberDto(m.role, m.userId))));
        return new AcademicCenterResponse(academicCenter);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<AcademicCenterResponse>> Update(
        Guid id, [FromBody] AcademicCenterUpdateBodyRequest request)
    {
        AcademicCenter academicCenter = await academicCenterService.UpdateAcademicCenter(id,
            new AcademicCenterUpdateDto(
                request.Name,
                request.AcademicCenterMembers.Select(m => new AcademicCenterMemberDto(m.role, m.userId)).ToList()));
        return new AcademicCenterResponse(academicCenter);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<NoContentResult> Delete(Guid id)
    {
        await academicCenterService.DeleteAcademicCenter(id);
        return new NoContentResult();
    }
}
