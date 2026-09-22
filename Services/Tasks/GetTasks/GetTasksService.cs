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

        return tasks.Select(TaskDtoMapper.ToDto).ToList();
    }
}
