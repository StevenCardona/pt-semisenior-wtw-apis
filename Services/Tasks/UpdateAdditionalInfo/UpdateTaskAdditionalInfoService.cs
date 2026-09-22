using Common.Exceptions;
using Repository.Tasks;
using Services.Tasks.Shared;

namespace Services.Tasks.UpdateAdditionalInfo;

public class UpdateTaskAdditionalInfoService
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskAdditionalInfoService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> ExecuteAsync(
        int taskId,
        UpdateTaskAdditionalInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var priority = TaskAdditionalInfoSerializer.NormalizePriorityFilter(request.Priority);

        var updated = await _taskRepository.PatchPriority(
            taskId,
            priority,
            request.UpdatedBy,
            cancellationToken);

        if (!updated)
        {
            throw new NotFoundException($"No existe una tarea con el id '{taskId}'.");
        }

        var task = await _taskRepository.GetById(taskId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"No existe una tarea con el id '{taskId}'.");
        }

        return TaskDtoMapper.ToDto(task);
    }
}
