using Common.Exceptions;
using Models.Tasks;
using Repository.Tasks;
using Services.Tasks.Shared;

namespace Services.Tasks.ChangeStatus;

public class ChangeTaskStatusService
{
    private readonly ITaskRepository _taskRepository;

    public ChangeTaskStatusService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> ExecuteAsync(
        int taskId,
        ChangeTaskStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetById(taskId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"No existe una tarea con el id '{taskId}'.");
        }

        task.ChangeStatus(request.Status);
        task.UpdatedBy = request.UpdatedBy;
        task.UpdatedDate = DateTime.UtcNow;

        await _taskRepository.Update(task, cancellationToken);

        var result = new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Status = task.Status,
            UserId = task.UserId,
            CreatedBy = task.CreatedBy,
            CreatedDate = task.CreatedDate,
            UpdatedBy = task.UpdatedBy,
            UpdatedDate = task.UpdatedDate
        };

        return result;
    }
}
