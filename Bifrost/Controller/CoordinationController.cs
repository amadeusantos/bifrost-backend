using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Coordination;
using Bifrost.Request;
using Bifrost.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost;

[Authorize]
[ApiController]
[Route("coordinations")]
public class CoordinationController(ICoordinationService coordinationService): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Coordination, CoordinationResponse>>> Paginate(
        [FromQuery] PaginationQueryRequest paginationQueryRequest,
        [FromQuery] Guid? assessmentSeasonId)
    {
        Pagination<Coordination> pagination =
            await coordinationService.GetCoordinations(paginationQueryRequest.Page, paginationQueryRequest.Size, assessmentSeasonId);

        return new PaginationResponse<Coordination, CoordinationResponse>(
            pagination, coordination => new CoordinationResponse(coordination));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoordinationResponse>> GetById(Guid id)
    {
        Coordination coordination = await coordinationService.GetCoordination(id);
        return new CoordinationResponse(coordination);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CoordinationResponse>> Create([FromBody] CoordinationCreateBodyRequest coordinationCreateBodyRequest)
    {
        Coordination coordination = await coordinationService.CreateCoordination(
            new CoordinationCreateDto(
                coordinationCreateBodyRequest.Name,
                coordinationCreateBodyRequest.AssessmentSeasonId,
                coordinationCreateBodyRequest.CoordinationMembers.Select(cm => new CoordinationMemberDto(cm.role, cm.userId))
                ));
        return new CoordinationResponse(coordination);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CoordinationResponse>> Update(
        Guid id, [FromBody] CoordinationUpdateBodyRequest coordinationUpdateBodyRequest)
    {
        Coordination coordination = await coordinationService.UpdateCoordination(id,
            new CoordinationUpdateDto(
                coordinationUpdateBodyRequest.Name,
                coordinationUpdateBodyRequest.CoordinationMembers
                    .Select(cm => new CoordinationMemberDto(cm.role, cm.userId)).ToList()
            ));
        return new CoordinationResponse(coordination);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<NoContentResult> Delete(Guid id)
    {
        await coordinationService.DeleteCoordination(id);
        return new NoContentResult();
    }
}