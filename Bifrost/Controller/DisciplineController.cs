using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Discipline;
using Bifrost.Request;
using Bifrost.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost;

[Authorize]
[ApiController]
[Route("disciplines")]
public class DisciplineController(IDisciplineService disciplineService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Discipline, DisciplineResponse>>> Paginate(
        [FromQuery] PaginationQueryRequest paginationQueryRequest)
    {
        Pagination<Discipline> pagination =
            await disciplineService.GetDisciplines(paginationQueryRequest.Page, paginationQueryRequest.Size);

        return new PaginationResponse<Discipline, DisciplineResponse>(
            pagination, d => new DisciplineResponse(d));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DisciplineResponse>> GetById(Guid id)
    {
        Discipline discipline = await disciplineService.GetDiscipline(id);
        return new DisciplineResponse(discipline);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DisciplineResponse>> Create([FromBody] DisciplineCreateBodyRequest request)
    {
        Discipline discipline = await disciplineService.CreateDiscipline(
            new DisciplineCreateDto(
                Guid.Empty,
                request.Name,
                request.Code,
                request.AssessmentSeasonId,
                request.ProfessorId,
                request.Students));
        return new DisciplineResponse(discipline);
    }
    
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DisciplineResponse>> Update(
        Guid id, [FromBody] DisciplineUpdateBodyRequest request)
    {
        Discipline discipline = await disciplineService.UpdateDiscipline(id,
            new DisciplineUpdateDto(
                id,
                request.Name,
                request.Code,
                request.ProfessorId,
                request.Students));
        return new DisciplineResponse(discipline);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<NoContentResult> Delete(Guid id)
    {
        await disciplineService.DeleteDiscipline(id);
        return new NoContentResult();
    }
}
