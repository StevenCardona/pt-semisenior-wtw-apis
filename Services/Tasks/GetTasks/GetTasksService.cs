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
        string? priority = null,
        CancellationToken cancellationToken = default)
    {
        string? normalizedPriority = null;

        if (!string.IsNullOrWhiteSpace(priority))
        {
            normalizedPriority = TaskAdditionalInfoSerializer.NormalizePriorityFilter(priority);
        }

        var tasks = await _taskRepository.GetAll(orderBy, normalizedPriority, cancellationToken);

        return tasks.Select(TaskDtoMapper.ToDto).ToList();
    }
}
