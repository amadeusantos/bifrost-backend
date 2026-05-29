using Bifrost.Core.Adapter;
using Bifrost.Core.Domain.Course;
using Bifrost.Request;
using Bifrost.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost;
[Authorize]
[ApiController]
[Route("courses")]
public class CourseController(ICourseService courseService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<CourseResponse>>> GetAll(
        )
    {
        var courses = await courseService.GetAllCoursesAsync();
        return Ok(courses.Select(DomainToResponse));
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Course, CourseResponse>>> Pagination(PaginationQueryRequest paginationQueryRequest)
    {
        var pageCourse = await courseService.PaginationCoursesAsync(paginationQueryRequest.Page, paginationQueryRequest.Size);
        var pageResponse = new PaginationResponse<Course,  CourseResponse>(
            pageCourse, 
            DomainToResponse
            );
        return Ok(pageResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseResponse>> GetById(Guid id)
    {
        var course = await courseService.GetCourseByIdAsync(id);
        return DomainToResponse(course);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CourseResponse>> Create(CourseBodyRequest body)
    {
        var courseDto = new CourseDto(Name: body.Name, Code: body.Code);
        var course = await courseService.CreateCourseAsync(courseDto);
        return DomainToResponse(course);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CourseResponse>> Update(Guid id, CourseBodyRequest body)
    {
        var courseDto = new CourseDto(Name: body.Name, Code: body.Code);
        var course = await courseService.UpdateCourseAsync(id, courseDto);
        return DomainToResponse(course);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<NoContentResult> Delete(Guid id)
    {
        await courseService.DeleteCourseByIdAsync(id);
        return NoContent();
    }

    private CourseResponse DomainToResponse(Course course)
    {
        return new CourseResponse(Id: course.Id, Name: course.Name, Code: course.Code);
    }
}