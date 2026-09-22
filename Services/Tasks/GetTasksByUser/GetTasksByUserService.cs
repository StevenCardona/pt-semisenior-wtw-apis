using Common.Exceptions;
using Models.Tasks;
using Repository.Tasks;
using Repository.Users;
using Services.Tasks.Shared;

namespace Services.Tasks.GetTasksByUser;

public class GetTasksByUserService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public GetTasksByUserService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> ExecuteAsync(
        int userId,
        TaskItemStatus? status = null,
        string? orderBy = null,
        string? priority = null,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetById(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException($"No existe un usuario con el id '{userId}'.");
        }

        string? normalizedPriority = null;

        if (!string.IsNullOrWhiteSpace(priority))
        {
            normalizedPriority = TaskAdditionalInfoSerializer.NormalizePriorityFilter(priority);
        }

        var tasks = await _taskRepository.GetByUser(
            userId,
            status,
            orderBy,
            normalizedPriority,
            cancellationToken);

        return tasks.Select(TaskDtoMapper.ToDto).ToList();
    }
}
