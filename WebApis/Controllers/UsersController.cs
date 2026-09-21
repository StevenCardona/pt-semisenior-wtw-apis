using Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Services.Users.CreateUser;
using Services.Users.GetUsers;
using Services.Users.Shared;

namespace WebApis.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly GetUsersService _getUsersService;
    private readonly CreateUserService _createUserService;

    public UsersController(GetUsersService getUsersService, CreateUserService createUserService)
    {
        _getUsersService = getUsersService;
        _createUserService = createUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDto>>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _getUsersService.ExecuteAsync(cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<UserDto>>
        {
            StatusCode = StatusCodes.Status200OK,
            Data = users,
            Messages = []
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _createUserService.ExecuteAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new ApiResponse<UserDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Data = user,
            Messages = []
        });
    }
}
