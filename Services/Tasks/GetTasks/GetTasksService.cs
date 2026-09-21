using Repository.Tasks;
using Services.Tasks.Shared;

namespace Services.Tasks.GetTasks;

public class GetTasksService
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> ExecuteAsync(
        string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetAll(orderBy, cancellationToken);

        var result = tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Status = t.Status,
            UserId = t.UserId,
            CreatedBy = t.CreatedBy,
            CreatedDate = t.CreatedDate,
            UpdatedBy = t.UpdatedBy,
            UpdatedDate = t.UpdatedDate
        }).ToList();

        return result;
    }
}
