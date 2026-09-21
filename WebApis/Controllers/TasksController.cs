using Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Models.Tasks;
using Services.Tasks.ChangeStatus;
using Services.Tasks.CreateTask;
using Services.Tasks.GetTasks;
using Services.Tasks.GetTasksByUser;
using Services.Tasks.Shared;

namespace WebApis.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly CreateTaskService _createTaskService;
    private readonly GetTasksService _getTasksService;
    private readonly ChangeTaskStatusService _changeTaskStatusService;
    private readonly GetTasksByUserService _getTasksByUserService;

    public TasksController(
        CreateTaskService createTaskService,
        GetTasksService getTasksService,
        ChangeTaskStatusService changeTaskStatusService,
        GetTasksByUserService getTasksByUserService)
    {
        _createTaskService = createTaskService;
        _getTasksService = getTasksService;
        _changeTaskStatusService = changeTaskStatusService;
        _getTasksByUserService = getTasksByUserService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskDto>>> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _createTaskService.ExecuteAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new ApiResponse<TaskDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Data = task,
            Messages = []
        });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TaskDto>>>> GetTasks(
        [FromQuery] string? orderBy,
        CancellationToken cancellationToken)
    {
        var tasks = await _getTasksService.ExecuteAsync(orderBy, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<TaskDto>>
        {
            StatusCode = StatusCodes.Status200OK,
            Data = tasks,
            Messages = []
        });
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> ChangeStatus(
        int id,
        [FromBody] ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _changeTaskStatusService.ExecuteAsync(id, request, cancellationToken);

        return Ok(new ApiResponse<TaskDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Data = task,
            Messages = []
        });
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TaskDto>>>> GetTasksByUser(
        int userId,
        [FromQuery] TaskItemStatus? status,
        [FromQuery] string? orderBy,
        CancellationToken cancellationToken)
    {
        var tasks = await _getTasksByUserService.ExecuteAsync(userId, status, orderBy, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<TaskDto>>
        {
            StatusCode = StatusCodes.Status200OK,
            Data = tasks,
            Messages = []
        });
    }
}
