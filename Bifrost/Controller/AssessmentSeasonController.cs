using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.AssessmentSeason;
using Bifrost.Request;
using Bifrost.Response;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost;

[ApiController]
[Route("assessment-seasons")]
public class AssessmentSeasonController(IAssessmentSeasonService assessmentSeasonService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<AssessmentSeasonResponse>>> GetAll()
    {
        List<AssessmentSeason> assessmentSeasons = await assessmentSeasonService.GetAllAssessmentSeasons();
        return assessmentSeasons.Select(a => new AssessmentSeasonResponse(a)).ToList();
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<AssessmentSeason, AssessmentSeasonResponse>>> Paginate(
        [FromQuery] PaginationQueryRequest paginationQueryRequest, [FromQuery(Name = "courseId")] Guid? courseId)
    {
        Pagination<AssessmentSeason> pagination =
            await assessmentSeasonService.GetAssessmentSeasons(paginationQueryRequest.Page, paginationQueryRequest.Size, courseId);

        return new PaginationResponse<AssessmentSeason, AssessmentSeasonResponse>(
            pagination, a => new AssessmentSeasonResponse(a));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssessmentSeasonResponse>> GetById(Guid id)
    {
        AssessmentSeason assessmentSeason = await assessmentSeasonService.GetAssessmentSeason(id);
        return new AssessmentSeasonResponse(assessmentSeason);
    }

    [HttpPost]
    public async Task<ActionResult<AssessmentSeasonResponse>> Create([FromBody] AssessmentSeasonCreateBodyRequest request)
    {
        AssessmentSeason assessmentSeason = await assessmentSeasonService.CreateAssessmentSeason(
            new AssessmentSeasonCreateDto(request.Period, request.CourseId, request.StartDateTime, request.EndDateTime));
        return new AssessmentSeasonResponse(assessmentSeason);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AssessmentSeasonResponse>> Update(
        Guid id, [FromBody] AssessmentSeasonUpdateBodyRequest request)
    {
        AssessmentSeason assessmentSeason = await assessmentSeasonService.UpdateAssessmentSeason(id,
            new AssessmentSeasonUpdateDto(request.Period, request.StartDateTime, request.EndDateTime));
        return new AssessmentSeasonResponse(assessmentSeason);
    }

    [HttpDelete("{id}")]
    public async Task<NoContentResult> Delete(Guid id)
    {
        await assessmentSeasonService.DeleteAssessmentSeason(id);
        return new NoContentResult();
    }
}
