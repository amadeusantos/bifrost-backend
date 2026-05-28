using Bifrost.Core.Adapter;
using Bifrost.Core.Domain;
using Bifrost.Core.Domain.Enum;
using Bifrost.Core.Domain.User;
using Bifrost.Request;
using Bifrost.Response;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost;

[ApiController]
[Route("users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<User, UserResponse>>> Paginate(
        [FromQuery] PaginationQueryRequest paginationQueryRequest,
        [FromQuery(Name = "profile")] UserProfileEnum? profile,
        [FromQuery(Name = "courseId")] Guid? courseId)
    {
        Pagination<User> pagination =
            await userService.GetUsers(paginationQueryRequest.Page, paginationQueryRequest.Size, profile, courseId);
        PaginationResponse<User, UserResponse> response =
            new PaginationResponse<User, UserResponse>(pagination, user => new UserResponse(user));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById([FromRoute] Guid id)
    {
        User user = await userService.GetUser(id);
        UserResponse response = new UserResponse(user);
        return Ok(response);
    }

    [HttpPost]
    public async Task<CreatedResult> Create([FromBody] UserCreateBodyRequest request)
    {
        User user = await userService.CreateUser(new UserCreateDto(
            Email: request.Email, 
            CourseId: request.CourseId, 
            Profile: request.Profile, 
            IsAdmin: request.IsAdmin, 
            FullName: request.FullName));
        UserResponse response = new UserResponse(user);
        return Created(response.Id.ToString(), response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> Update([FromRoute] Guid id, [FromBody] UserUpdateBodyRequest request)
    {
        User user = await userService.UpdateUser(id, new UserUpdateDto(
            FullName: request.FullName,
            Profile: request.Profile,
            IsAdmin: request.IsAdmin,
            CourseId: request.CourseId));
        return Ok(new UserResponse(user));
    }

    [HttpDelete("{id}")]
    public async Task<NoContentResult> Delete([FromRoute] Guid id)
    {
        await userService.DeleteUser(id);
        return NoContent();
    }
}